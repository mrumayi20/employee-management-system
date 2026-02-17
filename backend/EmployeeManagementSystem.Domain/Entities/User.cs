using EmployeeManagementSystem.Domain.Common;

namespace EmployeeManagementSystem.Domain.Entities;

public class User : BaseEntity
{
    public string FullName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string PasswordHash { get; set; } = default!;
    public string Role { get; set; } = "HR"; // "Admin" or "HR"
    public bool IsActive { get; set; } = true;
}