using System;

namespace employee_management_system.Models;

public class WorkLog
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int JobTaskId { get; set; }
    public JobTask JobTask { get; set; } = null!;

    public TimeSpan WorkStart { get; set; }
    public TimeSpan WorkEnd { get; set; }
}