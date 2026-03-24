namespace employee_management_system.Models;

public class Position
{
    public int Id { get; set; }
    public string PositionName { get; set; } = string.Empty;
    public decimal HourlyRate { get; set; }
}