namespace EmployeeManagementSystem.Application.Attendance;

public class UpdateAttendanceDto
{
    public int Status { get; set; } = 1;
    public TimeOnly? CheckIn { get; set; }
    public TimeOnly? CheckOut { get; set; }
}