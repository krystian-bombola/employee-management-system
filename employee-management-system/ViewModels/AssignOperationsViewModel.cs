using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using employee_management_system.Data;

namespace employee_management_system.ViewModels;

public partial class AssignOperationsViewModel : ViewModelBase
{
    public ObservableCollection<OperationItemViewModel> Operations { get; } = new();

    public Action? CloseAction { get; set; }

    public AssignOperationsViewModel()
    {
        using var db = new DatabaseContext();
        foreach (var op in db.Operations.ToList())
        {
            var vm = new OperationItemViewModel(op);
            vm.PropertyChanged += (_, __) => ConfirmCommand.NotifyCanExecuteChanged();
            Operations.Add(vm);
        }
    }

    [RelayCommand(CanExecute = nameof(CanConfirm))]
    private void Confirm() => CloseAction?.Invoke();

    private bool CanConfirm() => Operations.Any(o => o.IsSelected);

    [RelayCommand]
    private void Cancel()
    {
        foreach (var op in Operations) op.IsSelected = false;
        CloseAction?.Invoke();
    }
}
