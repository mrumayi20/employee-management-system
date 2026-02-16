using EmployeeManagementSystem.Domain.Common;

namespace EmployeeManagementSystem.Domain.Entities;

public class Department : BaseEntity
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }

    // Navigation
    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
}