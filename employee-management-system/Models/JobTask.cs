using System;
using System.Collections.Generic;

namespace employee_management_system.Models;

public class JobTask
{
    public int Id { get; set; }

    public int JobId { get; set; }
    public Job Job { get; set; } = null!;

    public int OperationId { get; set; }
    public Operation Operation { get; set; } = null!;

    public int Order { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime OperationStart { get; set; }
    public DateTime OperationEnd { get; set; }
    public TimeSpan ExecutionTime { get; set; }

    public ICollection<WorkLog> WorkLogs { get; set; } = new List<WorkLog>();
}