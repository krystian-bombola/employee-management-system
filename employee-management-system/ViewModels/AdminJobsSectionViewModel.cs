using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using employee_management_system.Data;
using employee_management_system.Models;
using employee_management_system.Repositories;
using employee_management_system.Services;
using Microsoft.EntityFrameworkCore;

namespace employee_management_system.ViewModels;

public partial class AdminJobsSectionViewModel : ViewModelBase
{
    private string _searchQuery = string.Empty;
    public string SearchQuery { get => _searchQuery; set { if (SetProperty(ref _searchQuery, value)) ApplyFilter(); } }

    [ObservableProperty] private bool _isDeleteConfirmationVisible;
    [ObservableProperty] private JobItemViewModel? _jobToDelete;
    [ObservableProperty] private JobItemViewModel? _selectedJob;

    [ObservableProperty] private bool _isAssignOperationsVisible;
    [ObservableProperty] private bool _isJobOperationsVisible;
    [ObservableProperty] private bool _isJobDetailsVisible;
    [ObservableProperty] private string _jobDetailsTitle = "";
    [ObservableProperty] private string _jobOperationsTitle = "";
    [ObservableProperty] private JobItemViewModel? _selectedJobForOperations;
    [ObservableProperty] private OperationItemViewModel? _selectedOperationToAdd;

    // Tymczasowe dane dla nowego zlecenia (przekazywane z dialogu)
    private string _tempNewJobName = string.Empty;
    private string _tempNewJobDescription = string.Empty;

    public ObservableCollection<JobItemViewModel> Jobs { get; } = new();
    public ObservableCollection<JobItemViewModel> FilteredJobs { get; } = new();
    public ObservableCollection<string> JobStatuses { get; } = new() { "Nowe", "W trakcie", "Zatrzymane", "Zakończone" };
    public ObservableCollection<OperationItemViewModel> AvailableOperationsForAssign { get; } = new();
    public ObservableCollection<JobOperationItemViewModel> JobOperations { get; } = new();
    public ObservableCollection<OperationItemViewModel> AvailableOperationsForJobEdit { get; } = new();
    public ObservableCollection<string> JobTaskStatuses { get; } = new() { "Nowe", "W trakcie", "Zakończone" };
    public ObservableCollection<JobOperationItemViewModel> JobDetailsOperations { get; } = new();

    public void Refresh(string? targetId = null)
    {
        using var db = new DatabaseContext();
        var jobService = new JobService(new JobRepository(db));
        Jobs.Clear();
        foreach (var job in jobService.GetAll())
        {
            Jobs.Add(new JobItemViewModel(job));
        }

        ApplyFilter();

        if (!string.IsNullOrEmpty(targetId))
        {
            SelectedJob = FilteredJobs.FirstOrDefault(j => j.JobName == targetId);
        }
    }

    private void ApplyFilter()
    {
        var query = SearchQuery.ToLower();
        FilteredJobs.Clear();
        foreach (var j in Jobs.Where(j => string.IsNullOrEmpty(query) ||
            j.JobName.ToLower().Contains(query) ||
            j.Description.ToLower().Contains(query) ||
            j.Status.ToLower().Contains(query)))
        {
            FilteredJobs.Add(j);
        }
    }

    [RelayCommand]
    private async Task AddJob()
    {
        var vm = new AddJobViewModel();
        var window = new Views.AddJobWindow(vm);

        if (Avalonia.Application.Current?.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop
            && desktop.MainWindow is not null)
        {
            await window.ShowDialog(desktop.MainWindow);
        }

        if (string.IsNullOrWhiteSpace(vm.JobName)) return; // Anulowano

        _tempNewJobName = vm.JobName;
        _tempNewJobDescription = vm.Description;

        // Teraz wybierz operacje
        AvailableOperationsForAssign.Clear();
        using var db = new DatabaseContext();
        foreach (var existing in db.Operations.ToList())
        {
            var op = new Operation { Id = existing.Id, OperationName = existing.OperationName, Description = existing.Description };
            var opVm = new OperationItemViewModel(op);
            opVm.PropertyChanged += (_, __) => ConfirmAddJobCommand.NotifyCanExecuteChanged();
            AvailableOperationsForAssign.Add(opVm);
        }

        IsAssignOperationsVisible = true;
    }

    [RelayCommand(CanExecute = nameof(CanConfirmAddJob))]
    private void ConfirmAddJob()
    {
        if (string.IsNullOrWhiteSpace(_tempNewJobName)) return;
        if (!AvailableOperationsForAssign.Any(o => o.IsSelected)) return;

        using var db = new DatabaseContext();
        var selectedNames = AvailableOperationsForAssign.Where(o => o.IsSelected).Select(o => o.OperationName).ToList();
        var selectedOpIds = db.Operations.Where(o => selectedNames.Contains(o.OperationName)).Select(o => o.Id).ToList();

        var jobService = new JobService(new JobRepository(db));
        jobService.Add(_tempNewJobName, _tempNewJobDescription, selectedOpIds, "Nowe");

        IsAssignOperationsVisible = false;
        _tempNewJobName = string.Empty;
        _tempNewJobDescription = string.Empty;
        Refresh();
    }

    private bool CanConfirmAddJob() => !string.IsNullOrWhiteSpace(_tempNewJobName) && AvailableOperationsForAssign.Any(o => o.IsSelected);

    [RelayCommand]
    private void CancelAddJob()
    {
        IsAssignOperationsVisible = false;
        _tempNewJobName = string.Empty;
        _tempNewJobDescription = string.Empty;
    }

    [RelayCommand]
    private void RemoveJob(JobItemViewModel? item)
    {
        var target = item ?? SelectedJob;
        if (target is null) return;

        JobToDelete = target;
        IsDeleteConfirmationVisible = true;
    }

    [RelayCommand]
    private void ConfirmDelete()
    {
        if (JobToDelete is null) return;

        // Pamięć wyboru
        string? nextTargetId = null;
        var index = FilteredJobs.IndexOf(JobToDelete);
        if (index >= 0)
        {
            if (index + 1 < FilteredJobs.Count) nextTargetId = FilteredJobs[index + 1].JobName;
            else if (index - 1 >= 0) nextTargetId = FilteredJobs[index - 1].JobName;
        }

        using var db = new DatabaseContext();
        var jobService = new JobService(new JobRepository(db));
        jobService.Remove(JobToDelete.JobName);

        IsDeleteConfirmationVisible = false;
        JobToDelete = null;
        Refresh(nextTargetId);
    }

    [RelayCommand]
    private void CancelDelete()
    {
        IsDeleteConfirmationVisible = false;
        JobToDelete = null;
    }

    [RelayCommand]
    private void OpenJobOperations(JobItemViewModel? item)
    {
        var target = item ?? SelectedJob;
        if (target is null) return;

        using var db = new DatabaseContext();
        var job = db.Jobs.FirstOrDefault(j => j.Id == target.Id);
        if (job is null) return;

        SelectedJobForOperations = target;
        JobOperationsTitle = $"Operacje dla zlecenia: {job.JobName}";

        AvailableOperationsForJobEdit.Clear();
        foreach (var op in db.Operations.OrderBy(o => o.OperationName).ToList())
            AvailableOperationsForJobEdit.Add(new OperationItemViewModel(op));

        JobOperations.Clear();
        var operations = db.JobTasks.Where(jt => jt.JobId == target.Id).Include(jt => jt.Operation).OrderBy(jt => jt.Order).ToList();
        foreach (var jt in operations)
            JobOperations.Add(new JobOperationItemViewModel(jt.Id, jt.OperationId, jt.Operation.OperationName, jt.Operation.Description, jt.Order, jt.Status, jt.ExecutionTime));

        NormalizeJobOperationOrder();
        SelectedOperationToAdd = null;
        IsJobOperationsVisible = true;
    }

    [RelayCommand]
    private void CloseJobOperations()
    {
        IsJobOperationsVisible = false;
        SelectedJobForOperations = null;
        SelectedOperationToAdd = null;
        JobOperations.Clear();
    }

    [RelayCommand]
    private void ShowJobDetails(JobItemViewModel? item)
    {
        if (item is null) return;
        using var db = new DatabaseContext();
        var job = db.Jobs.FirstOrDefault(j => j.Id == item.Id);
        if (job is null) return;

        JobDetailsTitle = $"Szczegóły zlecenia: {job.JobName}";
        JobDetailsOperations.Clear();
        var operations = db.JobTasks.Where(jt => jt.JobId == item.Id).Include(jt => jt.Operation).OrderBy(jt => jt.Order).ToList();
        foreach (var jt in operations)
            JobDetailsOperations.Add(new JobOperationItemViewModel(jt.Id, jt.OperationId, jt.Operation.OperationName, jt.Operation.Description, jt.Order, jt.Status, jt.ExecutionTime));

        IsJobDetailsVisible = true;
    }

    [RelayCommand]
    private void CloseJobDetails()
    {
        IsJobDetailsVisible = false;
        JobDetailsOperations.Clear();
    }

    [RelayCommand]
    private void AddOperationToJob()
    {
        if (SelectedOperationToAdd is null || JobOperations.Any(o => o.OperationId == SelectedOperationToAdd.Id)) return;
        JobOperations.Add(new JobOperationItemViewModel(0, SelectedOperationToAdd.Id, SelectedOperationToAdd.OperationName, SelectedOperationToAdd.Description, JobOperations.Count, "Nowe", TimeSpan.Zero));
        NormalizeJobOperationOrder();
    }

    [RelayCommand]
    private void RemoveOperationFromJob(JobOperationItemViewModel? item)
    {
        if (item is null) return;
        JobOperations.Remove(item);
        NormalizeJobOperationOrder();
    }

    [RelayCommand]
    private void MoveJobOperationUp(JobOperationItemViewModel? item)
    {
        if (item is null) return;
        var index = JobOperations.IndexOf(item);
        if (index <= 0) return;
        JobOperations.Move(index, index - 1);
        NormalizeJobOperationOrder();
    }

    [RelayCommand]
    private void MoveJobOperationDown(JobOperationItemViewModel? item)
    {
        if (item is null) return;
        var index = JobOperations.IndexOf(item);
        if (index < 0 || index >= JobOperations.Count - 1) return;
        JobOperations.Move(index, index + 1);
        NormalizeJobOperationOrder();
    }

    [RelayCommand]
    private void SaveJobOperations()
    {
        if (SelectedJobForOperations is null) return;

        using var db = new DatabaseContext();
        var existing = db.JobTasks.Where(jt => jt.JobId == SelectedJobForOperations.Id).ToList();
        var selectedOperationIds = JobOperations.Select(o => o.OperationId).ToList();

        var toDelete = existing.Where(jt => !selectedOperationIds.Contains(jt.OperationId)).ToList();
        if (toDelete.Count > 0) db.JobTasks.RemoveRange(toDelete);

        for (var i = 0; i < JobOperations.Count; i++)
        {
            var selected = JobOperations[i];
            var existingTask = existing.FirstOrDefault(jt => jt.OperationId == selected.OperationId);

            if (existingTask is null)
            {
                db.JobTasks.Add(new JobTask
                {
                    JobId = SelectedJobForOperations.Id,
                    OperationId = selected.OperationId,
                    Order = i,
                    Status = selected.Status,
                    OperationStart = DateTime.Now,
                    OperationEnd = DateTime.Now,
                    ExecutionTime = TimeSpan.Zero
                });
            }
            else
            {
                existingTask.Order = i;
                existingTask.Status = selected.Status;
            }
        }
        db.SaveChanges();

        var allTasks = db.JobTasks.Where(jt => jt.JobId == SelectedJobForOperations.Id).ToList();
        if (allTasks.Count > 0 && allTasks.All(jt => jt.Status == "Zakończone"))
        {
            var job = db.Jobs.FirstOrDefault(j => j.Id == SelectedJobForOperations.Id);
            if (job is not null)
            {
                job.Status = "Zakończone";
                db.SaveChanges();
            }
        }

        IsJobOperationsVisible = false;
        Refresh();
    }

    private void NormalizeJobOperationOrder()
    {
        for (var i = 0; i < JobOperations.Count; i++) JobOperations[i].Order = i;
    }
}
