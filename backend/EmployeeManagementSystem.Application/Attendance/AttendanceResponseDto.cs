namespace EmployeeManagementSystem.Application.Attendance;

public class AttendanceResponseDto
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = default!;
    public string EmployeeCode { get; set; } = default!;
    public string DepartmentName { get; set; } = default!;

    public DateOnly Date { get; set; }
    public string Status { get; set; } = default!;
    public TimeOnly? CheckIn { get; set; }
    public TimeOnly? CheckOut { get; set; }
}