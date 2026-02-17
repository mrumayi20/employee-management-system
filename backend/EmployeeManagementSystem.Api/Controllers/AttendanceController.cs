using EmployeeManagementSystem.Application.Attendance;
using EmployeeManagementSystem.Domain.Entities;
using EmployeeManagementSystem.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AttendanceController : ControllerBase
{
    private readonly AppDbContext _db;

    public AttendanceController(AppDbContext db) => _db = db;

    // GET /api/attendance?from=2026-02-01&to=2026-02-16&employeeId=...&departmentId=...
    [HttpGet]
    public async Task<ActionResult<List<AttendanceResponseDto>>> Get(
        [FromQuery] DateOnly? from,
        [FromQuery] DateOnly? to,
        [FromQuery] Guid? employeeId,
        [FromQuery] Guid? departmentId)
    {
        var query = _db.Attendances
            .AsNoTracking()
            .Include(a => a.Employee)
            .ThenInclude(e => e.Department)
            .AsQueryable();

        if (from.HasValue) query = query.Where(a => a.Date >= from.Value);
        if (to.HasValue) query = query.Where(a => a.Date <= to.Value);
        if (employeeId.HasValue) query = query.Where(a => a.EmployeeId == employeeId.Value);
        if (departmentId.HasValue) query = query.Where(a => a.Employee.DepartmentId == departmentId.Value);

        var data = await query
            .OrderByDescending(a => a.Date)
            .ThenBy(a => a.Employee.FirstName)
            .Select(a => new AttendanceResponseDto
            {
                Id = a.Id,
                EmployeeId = a.EmployeeId,
                EmployeeName = a.Employee.FirstName + " " + a.Employee.LastName,
                EmployeeCode = a.Employee.EmployeeCode,
                DepartmentName = a.Employee.Department.Name,
                Date = a.Date,
                Status = a.Status.ToString(),
                CheckIn = a.CheckIn,
                CheckOut = a.CheckOut
            })
            .ToListAsync();

        return Ok(data);
    }

    // GET /api/attendance/{id}
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AttendanceResponseDto>> GetById(Guid id)
    {
        var a = await _db.Attendances
            .AsNoTracking()
            .Include(x => x.Employee).ThenInclude(e => e.Department)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (a is null) return NotFound(new { error = "Attendance not found." });

        return Ok(new AttendanceResponseDto
        {
            Id = a.Id,
            EmployeeId = a.EmployeeId,
            EmployeeName = a.Employee.FirstName + " " + a.Employee.LastName,
            EmployeeCode = a.Employee.EmployeeCode,
            DepartmentName = a.Employee.Department.Name,
            Date = a.Date,
            Status = a.Status.ToString(),
            CheckIn = a.CheckIn,
            CheckOut = a.CheckOut
        });
    }

    // POST /api/attendance
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAttendanceDto dto)
    {
        // Validate employee
        var employeeExists = await _db.Employees.AnyAsync(e => e.Id == dto.EmployeeId);
        if (!employeeExists) return BadRequest(new { error = "Invalid EmployeeId." });

        // Validate status
        if (!Enum.IsDefined(typeof(AttendanceStatus), dto.Status))
            return BadRequest(new { error = "Invalid Status value." });

        var status = (AttendanceStatus)dto.Status;

        // Validate time logic
        if (dto.CheckIn.HasValue && dto.CheckOut.HasValue && dto.CheckOut < dto.CheckIn)
            return BadRequest(new { error = "CheckOut cannot be earlier than CheckIn." });

        // Enforce time rules for Absent/Leave
        if ((status == AttendanceStatus.Absent || status == AttendanceStatus.Leave) &&
            (dto.CheckIn.HasValue || dto.CheckOut.HasValue))
            return BadRequest(new { error = "CheckIn/CheckOut should be empty for Absent/Leave." });

        // Unique (employeeId + date) is enforced by DB index, but we return nice error
        var already = await _db.Attendances.AnyAsync(a => a.EmployeeId == dto.EmployeeId && a.Date == dto.Date);
        if (already) return Conflict(new { error = "Attendance already exists for this employee and date." });

        var attendance = new Attendance
        {
            EmployeeId = dto.EmployeeId,
            Date = dto.Date,
            Status = status,
            CheckIn = dto.CheckIn,
            CheckOut = dto.CheckOut
        };

        _db.Attendances.Add(attendance);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = attendance.Id }, new { attendance.Id });
    }

    // PUT /api/attendance/{id}
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAttendanceDto dto)
    {
        if (!Enum.IsDefined(typeof(AttendanceStatus), dto.Status))
            return BadRequest(new { error = "Invalid Status value." });

        if (dto.CheckIn.HasValue && dto.CheckOut.HasValue && dto.CheckOut < dto.CheckIn)
            return BadRequest(new { error = "CheckOut cannot be earlier than CheckIn." });

        var status = (AttendanceStatus)dto.Status;

        if ((status == AttendanceStatus.Absent || status == AttendanceStatus.Leave) &&
            (dto.CheckIn.HasValue || dto.CheckOut.HasValue))
            return BadRequest(new { error = "CheckIn/CheckOut should be empty for Absent/Leave." });

        var attendance = await _db.Attendances.FirstOrDefaultAsync(a => a.Id == id);
        if (attendance is null) return NotFound(new { error = "Attendance not found." });

        attendance.Status = status;
        attendance.CheckIn = dto.CheckIn;
        attendance.CheckOut = dto.CheckOut;
        attendance.UpdatedAtUtc = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    // DELETE /api/attendance/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var attendance = await _db.Attendances.FirstOrDefaultAsync(a => a.Id == id);
        if (attendance is null) return NotFound(new { error = "Attendance not found." });

        _db.Attendances.Remove(attendance);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}