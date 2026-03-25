
using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using employee_management_system.Data;
using System.Linq;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Microsoft.EntityFrameworkCore;

namespace employee_management_system.ViewModels;

public partial class UserPanelViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _mainVm;
    private readonly int _userId;

    [ObservableProperty]
    private string _employeeName = "---";

    [ObservableProperty]
    private string _orderId = "---";


    [ObservableProperty]
    private string _currentOperation = "Brak pracy";

    [ObservableProperty]
    private string? _selectedOperation;

    [ObservableProperty]
    private string? _selectedJob;

    [ObservableProperty]
    private bool _isOperationRunning;

    [ObservableProperty]
    private bool _isEndOperationConfirmationVisible;

    private string? _runningOperationName;
    private int? _runningJobId;

    public ObservableCollection<string> AvailableJobs { get; } = new();
    public ObservableCollection<string> AvailableOperations { get; } = new();

    public UserPanelViewModel(MainWindowViewModel mainVm, int userId, string employeeName)
    {
        _mainVm = mainVm;
        _userId = userId;
        _employeeName = employeeName;

        LoadJobsFromDb();
        LoadOperationsFromDb();
    }

    [RelayCommand]
    private void Logout()
    {
        _mainVm.CurrentView = new LoginViewModel(_mainVm);
    }

    [RelayCommand]
    private async System.Threading.Tasks.Task ChangePassword()
    {
        var vm = new ChangePasswordViewModel(_userId);
        var window = new Views.ChangePasswordWindow(vm);

        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
            && desktop.MainWindow is not null)
        {
            await window.ShowDialog(desktop.MainWindow);
        }
    }

    private void LoadJobsFromDb()
    {
        using var db = new DatabaseContext();
        AvailableJobs.Clear();

        var jobs = db.Jobs
            .Where(j => j.Status == "Nowe" || j.Status == "W produkcji" || j.Status == "W trakcie")
            .OrderBy(j => j.Id)
            .Select(j => new { j.Id, j.JobName, j.Description })
            .ToList();

        foreach (var job in jobs)
        {
            var display = string.IsNullOrWhiteSpace(job.Description)
                ? $"{job.Id} - {job.JobName}"
                : $"{job.Id} - {job.JobName} - {job.Description}";
            AvailableJobs.Add(display);
        }
    }

    private void LoadOperationsForJob(string? jobDisplay)
    {
        AvailableOperations.Clear();
        if (string.IsNullOrWhiteSpace(jobDisplay))
            return;

        var parts = jobDisplay.Split(" - ", 2, StringSplitOptions.None);
        if (!int.TryParse(parts[0], out var jobId))
            return;

        using var db = new DatabaseContext();
        var ops = db.JobTasks
                   .Where(jt => jt.JobId == jobId && jt.Status != "Zako\u0144czone")
                   .Include(jt => jt.Operation)
                   .OrderBy(jt => jt.Order)
                   .Select(jt => new { jt.Operation.OperationName, jt.Operation.Description, jt.Status })
                   .ToList();

        foreach (var op in ops)
        {
            var statusLabel = $"[{op.Status}]";
            var display = string.IsNullOrWhiteSpace(op.Description)
                ? $"{op.OperationName} {statusLabel}"
                : $"{op.OperationName} - {op.Description} {statusLabel}";
            AvailableOperations.Add(display);
        }
    }

    private void LoadOperationsFromDb()
    {
        using var db = new DatabaseContext();
        AvailableOperations.Clear();

        var operations = db.Operations
            .OrderBy(o => o.OperationName)
            .Select(o => new { o.OperationName, o.Description })
            .ToList();

        foreach (var op in operations)
        {
            var display = string.IsNullOrWhiteSpace(op.Description)
                ? op.OperationName
                : $"{op.OperationName} - {op.Description}";
            AvailableOperations.Add(display);
        }
    }

    [RelayCommand(CanExecute = nameof(CanStartOperation))]
    private void StartOperation()
    {
        if (SelectedOperation is null || SelectedJob is null) return;

        var operationName = GetOperationName(SelectedOperation);
        var jobName = GetJobName(SelectedJob);

        using (var db = new DatabaseContext())
        {
            var hasChanges = false;

            var op = db.Operations.FirstOrDefault(o => o.OperationName == operationName);
            if (op is not null)
            {
                op.CurrentWorkersCount++;
                hasChanges = true;
            }

            var jobId = GetJobId(SelectedJob);
            if (jobId is not null)
            {
                var job = db.Jobs.FirstOrDefault(j => j.Id == jobId.Value);
                if (job is not null && string.Equals(job.Status, "Nowe", StringComparison.OrdinalIgnoreCase))
                {
                    job.Status = "W produkcji";
                    hasChanges = true;
                }

                // Set JobTask status to "W trakcie"
                if (op is not null)
                {
                    var jobTask = db.JobTasks.FirstOrDefault(jt => jt.JobId == jobId.Value && jt.OperationId == op.Id);
                    if (jobTask is not null)
                    {
                        jobTask.Status = "W trakcie";
                        hasChanges = true;
                    }
                }
            }

            if (hasChanges)
            {
                db.SaveChanges();
            }
        }

        _runningOperationName = operationName;
        _runningJobId = GetJobId(SelectedJob);
        CurrentOperation = $"{jobName} - {operationName}";
        IsOperationRunning = true;
        StartOperationCommand.NotifyCanExecuteChanged();
        StopOperationCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand(CanExecute = nameof(CanStopOperation))]
    private void StopOperation()
    {
        StopRunningOperation();
        CurrentOperation = "Brak pracy";
    }

    [RelayCommand(CanExecute = nameof(CanStopOperation))]
    private void RequestEndOperation()
    {
        IsEndOperationConfirmationVisible = true;
    }

    [RelayCommand]
    private void CancelEndOperation()
    {
        IsEndOperationConfirmationVisible = false;
    }

    [RelayCommand]
    private void ConfirmEndOperation()
    {
        // Set JobTask status to "Zakończone" before stopping
        if (!string.IsNullOrEmpty(_runningOperationName) && _runningJobId is not null)
        {
            using var db = new DatabaseContext();
            var op = db.Operations.FirstOrDefault(o => o.OperationName == _runningOperationName);
            if (op is not null)
            {
                var jobTask = db.JobTasks.FirstOrDefault(jt => jt.JobId == _runningJobId.Value && jt.OperationId == op.Id);
                if (jobTask is not null)
                {
                    jobTask.Status = "Zako\u0144czone";
                    db.SaveChanges();
                }

                // Auto-complete job if all tasks are done
                var allTasks = db.JobTasks.Where(jt => jt.JobId == _runningJobId.Value).ToList();
                if (allTasks.Count > 0 && allTasks.All(jt => jt.Status == "Zako\u0144czone"))
                {
                    var job = db.Jobs.FirstOrDefault(j => j.Id == _runningJobId.Value);
                    if (job is not null)
                    {
                        job.Status = "Zako\u0144czone";
                        db.SaveChanges();
                    }
                }
            }
        }

        StopRunningOperation();
        CurrentOperation = "Brak pracy";
        IsEndOperationConfirmationVisible = false;
    }

    private void StopRunningOperation()
    {
        if (!string.IsNullOrEmpty(_runningOperationName))
        {
            using var db = new DatabaseContext();
            var op = db.Operations.FirstOrDefault(o => o.OperationName == _runningOperationName);
            if (op is not null && op.CurrentWorkersCount > 0)
            {
                op.CurrentWorkersCount--;
                db.SaveChanges();
            }
        }

        _runningOperationName = null;
        IsOperationRunning = false;
    }

    private static string GetJobName(string selectedJob)
    {
        var parts = selectedJob.Split(" - ", 3, StringSplitOptions.None);
        return parts.Length > 1 ? parts[1] : selectedJob;
    }

    private static string GetOperationName(string selectedOperation)
    {
        var parts = selectedOperation.Split(" - ", 2, StringSplitOptions.None);
        return parts[0];
    }

    private static int? GetJobId(string selectedJob)
    {
        var parts = selectedJob.Split(" - ", 3, StringSplitOptions.None);
        if (parts.Length == 0)
        {
            return null;
        }

        return int.TryParse(parts[0], out var jobId) ? jobId : null;
    }

    public bool AreSelectorsEnabled => !IsOperationRunning;

    private bool CanStartOperation() => !IsOperationRunning && SelectedOperation is not null && SelectedJob is not null;
    private bool CanStopOperation() => IsOperationRunning;

    partial void OnSelectedJobChanged(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            OrderId = "---";
            AvailableOperations.Clear();
            StartOperationCommand.NotifyCanExecuteChanged();
            return;
        }

        var parts = value.Split(" - ", 2, StringSplitOptions.None);
        OrderId = parts.Length > 0 ? parts[0] : value;
        LoadOperationsForJob(value);
        StartOperationCommand.NotifyCanExecuteChanged();
    }

    partial void OnSelectedOperationChanged(string? value)
    {
        StartOperationCommand.NotifyCanExecuteChanged();
    }

    partial void OnIsOperationRunningChanged(bool value)
    {
        OnPropertyChanged(nameof(AreSelectorsEnabled));
        StartOperationCommand.NotifyCanExecuteChanged();
        StopOperationCommand.NotifyCanExecuteChanged();
        RequestEndOperationCommand.NotifyCanExecuteChanged();
        if (!value)
        {
            IsEndOperationConfirmationVisible = false;
        }
    }
}