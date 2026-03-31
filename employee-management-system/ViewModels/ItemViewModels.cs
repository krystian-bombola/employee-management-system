using System;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using employee_management_system.Models;

namespace employee_management_system.ViewModels;

public partial class UserItemViewModel : ObservableObject
{
    [ObservableProperty] private bool _isSelected;
    public int Id { get; }
    public string FirstName { get; }
    public string LastName { get; }
    public string Identifier { get; }
    public string EmploymentDate { get; }
    public string PositionName { get; }

    public UserItemViewModel(User user)
    {
        Id = user.Id;
        FirstName = user.FirstName;
        LastName = user.LastName;
        Identifier = user.Identifier;
        EmploymentDate = user.EmploymentDate;
        PositionName = user.Position?.PositionName ?? "—";
    }
}

public partial class OperationItemViewModel : ObservableObject
{
    [ObservableProperty] private bool _isSelected;
    public int Id { get; }
    public string OperationName { get; }
    public string Description { get; }

    public OperationItemViewModel(Operation op)
    {
        Id = op.Id;
        OperationName = op.OperationName;
        Description = op.Description;
    }
}

public partial class JobOperationItemViewModel : ObservableObject
{
    public int JobTaskId { get; }
    public int OperationId { get; }
    public string OperationName { get; }
    public string Description { get; }

    [ObservableProperty] private int _order;
    [ObservableProperty] private string _status;
    
    public TimeSpan ExecutionTime { get; }
    public double ExecutionTimeHours => ExecutionTime.TotalHours;

    public JobOperationItemViewModel(int jobTaskId, int operationId, string operationName, string description, int order, string status, TimeSpan executionTime = default)
    {
        JobTaskId = jobTaskId;
        OperationId = operationId;
        OperationName = operationName;
        Description = description;
        _order = order;
        _status = status;
        ExecutionTime = executionTime;
    }
}

public partial class OperationSelectionItemViewModel : ObservableObject
{
    [ObservableProperty] private bool _isSelected;
    public int Id { get; }
    public string OperationName { get; }
    public string Description { get; }

    public OperationSelectionItemViewModel(Operation op)
    {
        Id = op.Id;
        OperationName = op.OperationName;
        Description = op.Description;
    }
}

public partial class PositionItemViewModel : ObservableObject
{
    [ObservableProperty] private bool _isSelected;
    public int Id { get; }
    public string PositionName { get; }
    public decimal HourlyRate { get; }

    public PositionItemViewModel(Position position)
    {
        Id = position.Id;
        PositionName = position.PositionName;
        HourlyRate = position.HourlyRate;
    }
}

public partial class JobItemViewModel : ObservableObject
{
    [ObservableProperty] private bool _isSelected;
    public int Id { get; }
    public string JobName { get; }
    public string Description { get; }
    public string Status { get; }
    public DateTime CreatedAt { get; }
    public double TotalExecutionTimeHours { get; }

    public JobItemViewModel(Job job)
    {
        Id = job.Id;
        JobName = job.JobName;
        Description = job.Description;
        Status = job.Status;
        CreatedAt = job.CreatedAt;
        TotalExecutionTimeHours = job.JobTasks?.Sum(jt => jt.ExecutionTime.TotalHours) ?? 0;
    }
}
