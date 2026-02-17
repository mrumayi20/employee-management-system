namespace EmployeeManagementSystem.Application.Attendance;

public class CreateAttendanceDto
{
    public Guid EmployeeId { get; set; }
    public DateOnly Date { get; set; }

    // 1=Present, 2=Absent, 3=HalfDay, 4=Leave
    public int Status { get; set; } = 1;

    public TimeOnly? CheckIn { get; set; }
    public TimeOnly? CheckOut { get; set; }
}