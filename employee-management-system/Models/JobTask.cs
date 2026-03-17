using System;
using System.Collections.Generic;

public class JobTask
{
    public int Id { get; set; }

    public int JobId { get; set; }
    public Job Job { get; set; }

    public int OperationId { get; set; }
    public Operation Operation { get; set; }

    public int Order { get; set; }
    public DateTime OperationStart { get; set; }
    public DateTime OperationEnd { get; set; }
    public TimeSpan ExecutionTime { get; set; }

    public ICollection<WorkLog> WorkLogs { get; set; } = new List<WorkLog>();
}