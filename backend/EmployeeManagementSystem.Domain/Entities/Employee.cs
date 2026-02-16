using EmployeeManagementSystem.Domain.Common;

namespace EmployeeManagementSystem.Domain.Entities;

public class Employee : BaseEntity
{
    public string EmployeeCode { get; set; } = default!;   // human-readable id (EMP0001)
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Phone { get; set; } = default!;
    public DateTime DateOfJoining { get; set; }
    public bool IsActive { get; set; } = true;

    // FK
    public Guid DepartmentId { get; set; }
    public Department Department { get; set; } = default!;

    // Navigation
    public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
    public ICollection<SalaryRecord> SalaryRecords { get; set; } = new List<SalaryRecord>();
}