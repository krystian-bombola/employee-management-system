using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using employee_management_system.Data;
using employee_management_system.Models;
using Microsoft.EntityFrameworkCore;

namespace employee_management_system.ViewModels;

public partial class EditJobOperationsViewModel : ViewModelBase
{
    private readonly int _jobId;

    public string Title { get; }

    public ObservableCollection<JobOperationItemViewModel> Operations { get; } = new();
    public ObservableCollection<OperationItemViewModel> AvailableOperations { get; } = new();
    public ObservableCollection<string> TaskStatuses { get; } = new() { "Nowe", "W trakcie", "Zakończone" };

    [ObservableProperty] private OperationItemViewModel? _selectedOperationToAdd;

    private readonly System.Collections.Generic.List<OperationItemViewModel> _allOperations = new();

    public Action? CloseAction { get; set; }
    public Action? OnSavedAction { get; set; }

    public EditJobOperationsViewModel(int jobId, string jobName)
    {
        _jobId = jobId;
        Title = $"Operacje dla zlecenia: {jobName}";

        LoadData();
    }

    private void LoadData()
    {
        using var db = new DatabaseContext();
        
        _allOperations.Clear();
        foreach (var op in db.Operations.OrderBy(o => o.OperationName).ToList())
            _allOperations.Add(new OperationItemViewModel(op));

        Operations.Clear();
        var operations = db.JobTasks.Where(jt => jt.JobId == _jobId).Include(jt => jt.Operation).OrderBy(jt => jt.Order).ToList();
        foreach (var jt in operations)
            Operations.Add(new JobOperationItemViewModel(jt.Id, jt.OperationId, jt.Operation.OperationName, jt.Operation.Description, jt.Order, jt.Status, jt.ExecutionTime));

        NormalizeOrder();
        UpdateAvailableOperations();
    }

    private void UpdateAvailableOperations()
    {
        var currentIds = Operations.Select(o => o.OperationId).ToHashSet();
        AvailableOperations.Clear();
        foreach (var op in _allOperations.Where(o => !currentIds.Contains(o.Id)))
        {
            AvailableOperations.Add(op);
        }
        SelectedOperationToAdd = AvailableOperations.FirstOrDefault();
    }

    [RelayCommand]
    private void AddOperation()
    {
        if (SelectedOperationToAdd is null || Operations.Any(o => o.OperationId == SelectedOperationToAdd.Id)) return;
        Operations.Add(new JobOperationItemViewModel(0, SelectedOperationToAdd.Id, SelectedOperationToAdd.OperationName, SelectedOperationToAdd.Description, Operations.Count, "Nowe", TimeSpan.Zero));
        NormalizeOrder();
        UpdateAvailableOperations();
    }

    [RelayCommand]
    private void RemoveOperation(JobOperationItemViewModel? item)
    {
        if (item is null) return;
        Operations.Remove(item);
        NormalizeOrder();
        UpdateAvailableOperations();
    }

    [RelayCommand]
    private void MoveUp(JobOperationItemViewModel? item)
    {
        if (item is null) return;
        var index = Operations.IndexOf(item);
        if (index <= 0) return;
        Operations.Move(index, index - 1);
        NormalizeOrder();
    }

    [RelayCommand]
    private void MoveDown(JobOperationItemViewModel? item)
    {
        if (item is null) return;
        var index = Operations.IndexOf(item);
        if (index < 0 || index >= Operations.Count - 1) return;
        Operations.Move(index, index + 1);
        NormalizeOrder();
    }

    [RelayCommand]
    private void Save()
    {
        using var db = new DatabaseContext();
        var existing = db.JobTasks.Where(jt => jt.JobId == _jobId).ToList();
        var selectedOperationIds = Operations.Select(o => o.OperationId).ToList();

        var toDelete = existing.Where(jt => !selectedOperationIds.Contains(jt.OperationId)).ToList();
        if (toDelete.Count > 0) db.JobTasks.RemoveRange(toDelete);

        for (var i = 0; i < Operations.Count; i++)
        {
            var selected = Operations[i];
            var existingTask = existing.FirstOrDefault(jt => jt.OperationId == selected.OperationId);

            if (existingTask is null)
            {
                db.JobTasks.Add(new JobTask
                {
                    JobId = _jobId,
                    OperationId = selected.OperationId,
                    Order = i,
                    Status = string.IsNullOrWhiteSpace(selected.Status) ? "Nowe" : selected.Status,
                    OperationStart = DateTime.Now,
                    OperationEnd = DateTime.Now,
                    ExecutionTime = TimeSpan.Zero
                });
            }
            else
            {
                existingTask.Order = i;
                existingTask.Status = string.IsNullOrWhiteSpace(selected.Status) ? "Nowe" : selected.Status;
            }
        }
        db.SaveChanges();

        var allTasks = db.JobTasks.Where(jt => jt.JobId == _jobId).ToList();
        if (allTasks.Count > 0 && allTasks.All(jt => jt.Status == "Zakończone"))
        {
            var job = db.Jobs.FirstOrDefault(j => j.Id == _jobId);
            if (job is not null && job.Status != "Zakończone")
            {
                job.Status = "Zakończone";
                db.SaveChanges();
            }
        }

        OnSavedAction?.Invoke();
        CloseAction?.Invoke();
    }

    [RelayCommand]
    private void Cancel()
    {
        CloseAction?.Invoke();
    }

    private void NormalizeOrder()
    {
        for (var i = 0; i < Operations.Count; i++) Operations[i].Order = i;
    }
}
