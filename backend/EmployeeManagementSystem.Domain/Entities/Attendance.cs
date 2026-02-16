using EmployeeManagementSystem.Domain.Common;

namespace EmployeeManagementSystem.Domain.Entities;

public class Attendance : BaseEntity
{
    public Guid EmployeeId { get; set; }
    public Employee Employee { get; set; } = default!;

    public DateOnly Date { get; set; }
    public TimeOnly? CheckIn { get; set; }
    public TimeOnly? CheckOut { get; set; }

    public AttendanceStatus Status { get; set; } = AttendanceStatus.Present;
}

public enum AttendanceStatus
{
    Present = 1,
    Absent = 2,
    HalfDay = 3,
    Leave = 4
}