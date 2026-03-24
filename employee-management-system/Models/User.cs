using System;
using System.Collections.Generic;

namespace employee_management_system.Models;

public class User
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Identifier { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;
    public string PasswordSalt { get; set; } = string.Empty;

    public bool IsAdmin { get; set; } = false;
    public string EmploymentDate { get; set; } = System.DateTime.Now.ToString("yyyy-MM-dd");
    public int? PositionId { get; set; }
    public Position? Position { get; set; }
}