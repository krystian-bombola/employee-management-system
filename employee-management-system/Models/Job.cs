using System;
using System.Collections.Generic;

namespace employee_management_system.Models;

public class Job
{
    public int Id { get; set; }
    public string JobName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string Status { get; set; } = string.Empty;

    public ICollection<JobTask> JobTasks { get; set; } = new List<JobTask>();
}