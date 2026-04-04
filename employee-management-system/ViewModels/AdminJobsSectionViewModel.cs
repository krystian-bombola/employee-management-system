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

    [ObservableProperty] private bool _isJobDetailsVisible;
    [ObservableProperty] private string _jobDetailsTitle = "";

    public ObservableCollection<JobItemViewModel> Jobs { get; } = new();
    public ObservableCollection<JobItemViewModel> FilteredJobs { get; } = new();
    public ObservableCollection<string> JobStatuses { get; } = new() { "Nowe", "W trakcie", "Zatrzymane", "Zakończone" };
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
        var mainWindow = GetMainWindow();
        if (mainWindow is null) return;

        // Krok 1: Nazwa i opis
        var step1Vm = new AddJobViewModel();
        var step1Win = new Views.AddJobWindow(step1Vm);
        await step1Win.ShowDialog(mainWindow);
        if (string.IsNullOrWhiteSpace(step1Vm.JobName)) return;

        // Krok 2: Wybierz operacje
        var step2Vm = new AssignOperationsViewModel();
        var step2Win = new Views.AssignOperationsWindow(step2Vm);
        await step2Win.ShowDialog(mainWindow);
        var selected = step2Vm.Operations.Where(o => o.IsSelected).ToList();
        if (!selected.Any()) return;

        // Krok 3: Ustaw priorytet
        var tempOps = selected.Select((o, i) =>
            new JobOperationItemViewModel(0, o.Id, o.OperationName, o.Description, i, "Nowe", TimeSpan.Zero));
        var step3Vm = new SetPriorityViewModel(tempOps);
        var step3Win = new Views.SetPriorityWindow(step3Vm);
        await step3Win.ShowDialog(mainWindow);
        if (!step3Vm.Confirmed) return;

        // Zapisz
        var orderedOpIds = step3Vm.Operations.OrderBy(o => o.Order).Select(o => o.OperationId).ToList();
        using var db = new DatabaseContext();
        var jobService = new JobService(new JobRepository(db));
        jobService.Add(step1Vm.JobName, step1Vm.Description, orderedOpIds, "Nowe");
        Refresh();
    }

    private static Avalonia.Controls.Window? GetMainWindow() =>
        (Avalonia.Application.Current?.ApplicationLifetime
            as Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime)
            ?.MainWindow;

    [RelayCommand]
    private async Task EditJob(JobItemViewModel? item)
    {
        var target = item ?? SelectedJob;
        if (target is null) return;

        var mainWindow = GetMainWindow();
        if (mainWindow is null) return;

        var editVm = new AddJobViewModel
        {
            JobName = target.JobName,
            Description = target.Description,
            WindowTitle = "Edytuj zlecenie",
            HeaderText = "Edytuj nazwę i opis",
            ConfirmButtonText = "Zapisz"
        };
        var editWin = new Views.AddJobWindow(editVm);
        await editWin.ShowDialog(mainWindow);

        if (string.IsNullOrWhiteSpace(editVm.JobName)) return;

        using var db = new DatabaseContext();
        var jobService = new JobService(new JobRepository(db));
        jobService.UpdateJobNameAndDescription(target.Id, editVm.JobName, editVm.Description);
        
        Refresh();
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
    private async Task OpenJobOperations(JobItemViewModel? item)
    {
        var target = item ?? SelectedJob;
        if (target is null) return;

        var mainWindow = GetMainWindow();
        if (mainWindow is null) return;

        var editVm = new EditJobOperationsViewModel(target.Id, target.JobName);
        var editWin = new Views.EditJobOperationsWindow(editVm);
        
        editVm.OnSavedAction = () => Refresh();

        await editWin.ShowDialog(mainWindow);
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
}
