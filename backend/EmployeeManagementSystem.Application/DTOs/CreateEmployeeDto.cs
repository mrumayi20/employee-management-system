namespace EmployeeManagementSystem.Application.DTOs;

public class CreateEmployeeDto
{
    public string EmployeeCode { get; set; } = default!;
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Phone { get; set; } = default!;
    public DateTime DateOfJoining { get; set; }
    public Guid DepartmentId { get; set; }
}