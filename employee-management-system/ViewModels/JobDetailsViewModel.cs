using System;
using System.Collections.ObjectModel;

namespace employee_management_system.ViewModels;

public class JobDetailsViewModel : ViewModelBase
{
    public string Title { get; }
    public ObservableCollection<JobOperationItemViewModel> Operations { get; } = new();
    public Action? CloseAction { get; set; }

    public JobDetailsViewModel(string title, System.Collections.Generic.IEnumerable<JobOperationItemViewModel> operations)
    {
        Title = title;
        foreach (var operation in operations)
        {
            Operations.Add(operation);
        }
    }

    public void Close() => CloseAction?.Invoke();
}
