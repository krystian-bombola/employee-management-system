
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using employee_management_system.Data;
using System.Linq;

namespace employee_management_system.ViewModels;

public partial class UserPanelViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _mainVm;

    [ObservableProperty]
    private string _employeeName = "---";

    [ObservableProperty]
    private string _orderId = "---";

    [ObservableProperty]
    private string _currentOperation = "No operation";

    [ObservableProperty]
    private string? _selectedOperation;

    [ObservableProperty]
    private bool _isOperationRunning;

    private string? _runningOperationName;

    public ObservableCollection<string> AvailableOperations { get; } = new();

    public UserPanelViewModel(MainWindowViewModel mainVm, string employeeName)
    {
        _mainVm = mainVm;
        _employeeName = employeeName;

        LoadOperationsFromDb();
    }

    [RelayCommand]
    private void Logout()
    {
        _mainVm.CurrentView = new LoginViewModel(_mainVm);
    }

    private void LoadOperationsFromDb()
    {
        using var db = new DatabaseContext();
        AvailableOperations.Clear();

        var operations = db.Operations.Select(o => o.OperationName).ToList();
        foreach (var op in operations)
            AvailableOperations.Add(op);
    }

    [RelayCommand(CanExecute = nameof(CanStartOperation))]
    private void StartOperation()
    {
        if (SelectedOperation is null) return;

        using (var db = new DatabaseContext())
        {
            var op = db.Operations.FirstOrDefault(o => o.OperationName == SelectedOperation);
            if (op is not null)
            {
                op.CurrentWorkersCount++;
                db.SaveChanges();
            }
        }

        _runningOperationName = SelectedOperation;
        CurrentOperation = SelectedOperation;
        IsOperationRunning = true;
        StartOperationCommand.NotifyCanExecuteChanged();
        StopOperationCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand(CanExecute = nameof(CanStopOperation))]
    private void StopOperation()
    {
        if (!string.IsNullOrEmpty(_runningOperationName))
        {
            using (var db = new DatabaseContext())
            {
                var op = db.Operations.FirstOrDefault(o => o.OperationName == _runningOperationName);
                if (op is not null && op.CurrentWorkersCount > 0)
                {
                    op.CurrentWorkersCount--;
                    db.SaveChanges();
                }
            }
        }

        _runningOperationName = null;
        CurrentOperation = "No operation";
        IsOperationRunning = false;
        StartOperationCommand.NotifyCanExecuteChanged();
        StopOperationCommand.NotifyCanExecuteChanged();
    }

    private bool CanStartOperation() => !IsOperationRunning && SelectedOperation is not null;
    private bool CanStopOperation() => IsOperationRunning;

    partial void OnSelectedOperationChanged(string? value)
    {
        StartOperationCommand.NotifyCanExecuteChanged();
    }
}