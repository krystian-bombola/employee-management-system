using System;

namespace employee_management_system.Models;

public class WorkLog
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public User User { get; set; }

    public int JobTaskId { get; set; }
    public JobTask JobTask { get; set; }

    public TimeSpan WorkStart { get; set; }
    public TimeSpan WorkEnd { get; set; }
}