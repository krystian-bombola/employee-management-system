using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using employee_management_system.Models;
using employee_management_system.Data;

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
    public bool IsAdmin { get; }

    public UserItemViewModel(User user)
    {
        Id = user.Id;
        FirstName = user.FirstName;
        LastName = user.LastName;
        Identifier = user.Identifier;
        EmploymentDate = user.EmploymentDate;
        PositionName = user.Position?.PositionName ?? "—";
        IsAdmin = user.IsAdmin;
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

    [RelayCommand]
    private void ToggleSelection() => IsSelected = !IsSelected;
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

    public double ComputedCost { get; }
    public string ComputedCostFormatted => Math.Round(ComputedCost, 2) <= 0 ? "—" : $"{ComputedCost:F2} zł";

    public JobItemViewModel(Job job)
    {
        Id = job.Id;
        JobName = job.JobName;
        Description = job.Description;
        Status = job.Status;
        CreatedAt = job.CreatedAt;
        TotalExecutionTimeHours = job.JobTasks?.Sum(jt => jt.ExecutionTime.TotalHours) ?? 0;

        try
        {
            using var db = new DatabaseContext();

            double defaultHourlyRate = 40.0;
            if (db.Positions.Any())
            {
                defaultHourlyRate = (double)db.Positions.Average(p => p.HourlyRate);
            }

            double labor = 0.0;
            double totalHours = 0.0;

            var tasks = db.JobTasks.Where(jt => jt.JobId == job.Id).ToList();
            foreach (var t in tasks)
            {
                var wls = db.WorkLogs.Where(w => w.JobTaskId == t.Id)
                    .Include(w => w.User)
                    .ThenInclude(u => u.Position)
                    .ToList();

                if (wls.Any())
                {
                    foreach (var wl in wls)
                    {
                        var duration = (wl.WorkEnd - wl.WorkStart).TotalHours;
                        if (duration <= 0) continue;
                        double rate = defaultHourlyRate;
                        if (wl.User?.Position != null) rate = (double)wl.User.Position.HourlyRate;
                        labor += duration * rate;
                        totalHours += duration;
                    }
                }
                else
                {
                    var execHours = t.ExecutionTime.TotalHours;
                    if (execHours > 0)
                    {
                        labor += execHours * defaultHourlyRate;
                        totalHours += execHours;
                    }
                }
            }

            double additionalPerHour = 220.0; 
            ComputedCost = labor + (totalHours * additionalPerHour);
        }
        catch
        {
            ComputedCost = 0.0;
        }
    }
}
