using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace employee_management_system.ViewModels;

public partial class UserPanelViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _employeeName = "---";

    [ObservableProperty]
    private string _orderId = "---";

    [ObservableProperty]
    private string _currentOperation = "brak operacji";

    [ObservableProperty]
    private string? _selectedOperation;

    [ObservableProperty]
    private bool _isOperationRunning;

    public ObservableCollection<string> AvailableOperations { get; } = new()
    {
        "Montaż",
        "Spawanie",
        "Kontrola jakości",
        "Pakowanie",
        "Transport"
    };

    public UserPanelViewModel()
    {
    }

    public UserPanelViewModel(string employeeName, string orderId)
    {
        _employeeName = employeeName;
        _orderId = orderId;
    }

    [RelayCommand(CanExecute = nameof(CanStartOperation))]
    private void StartOperation()
    {
        if (SelectedOperation is null) return;

        CurrentOperation = SelectedOperation;
        IsOperationRunning = true;
        StartOperationCommand.NotifyCanExecuteChanged();
        StopOperationCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand(CanExecute = nameof(CanStopOperation))]
    private void StopOperation()
    {
        CurrentOperation = "brak operacji";
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
