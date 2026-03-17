using System.Collections.Generic;

using System;

namespace employee_management_system.Models;

public class Operation
{
    public int Id { get; set; }
    public string OperationName { get; set; } = null!;
    public string Description { get; set; } = string.Empty;


    public int CurrentWorkersCount { get; set; } = 0;

    public ICollection<JobTask> JobTasks { get; set; } = new List<JobTask>();
}