using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;

namespace employee_management_system.ViewModels;

public partial class SetPriorityViewModel : ViewModelBase
{
    public ObservableCollection<JobOperationItemViewModel> Operations { get; } = new();

    public Action? CloseAction { get; set; }
    public bool Confirmed { get; private set; } = false;

    public SetPriorityViewModel(IEnumerable<JobOperationItemViewModel> operations)
    {
        foreach (var op in operations)
            Operations.Add(op);
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
    private void Confirm()
    {
        Confirmed = true;
        CloseAction?.Invoke();
    }

    [RelayCommand]
    private void Cancel()
    {
        Confirmed = false;
        CloseAction?.Invoke();
    }

    private void NormalizeOrder()
    {
        for (var i = 0; i < Operations.Count; i++) Operations[i].Order = i;
    }
}
