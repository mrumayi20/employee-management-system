namespace EmployeeManagementSystem.Application.DTOs;

public class UpdateEmployeeDto
{
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string Phone { get; set; } = default!;
    public bool IsActive { get; set; }
}