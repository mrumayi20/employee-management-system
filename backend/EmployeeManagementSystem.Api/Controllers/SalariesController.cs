using EmployeeManagementSystem.Application.Salary;
using EmployeeManagementSystem.Domain.Entities;
using EmployeeManagementSystem.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SalariesController : ControllerBase
{
    private readonly AppDbContext _db;
    public SalariesController(AppDbContext db) => _db = db;

    // GET /api/salaries?employeeId=...&departmentId=...&year=2026&month=2
    [HttpGet]
    public async Task<ActionResult<List<SalaryResponseDto>>> Get(
        [FromQuery] Guid? employeeId,
        [FromQuery] Guid? departmentId,
        [FromQuery] int? year,
        [FromQuery] int? month)
    {
        var query = _db.SalaryRecords
            .AsNoTracking()
            .Include(s => s.Employee)
            .ThenInclude(e => e.Department)
            .AsQueryable();

        if (employeeId.HasValue) query = query.Where(s => s.EmployeeId == employeeId.Value);
        if (departmentId.HasValue) query = query.Where(s => s.Employee.DepartmentId == departmentId.Value);
        if (year.HasValue) query = query.Where(s => s.Year == year.Value);
        if (month.HasValue) query = query.Where(s => s.Month == month.Value);

        var data = await query
            .OrderByDescending(s => s.Year)
            .ThenByDescending(s => s.Month)
            .ThenBy(s => s.Employee.FirstName)
            .Select(s => new SalaryResponseDto
            {
                Id = s.Id,
                EmployeeId = s.EmployeeId,
                EmployeeName = s.Employee.FirstName + " " + s.Employee.LastName,
                EmployeeCode = s.Employee.EmployeeCode,
                DepartmentName = s.Employee.Department.Name,
                Year = s.Year,
                Month = s.Month,
                Basic = s.Basic,
                Allowances = s.Allowances,
                Deductions = s.Deductions,
                NetPay = s.Basic + s.Allowances - s.Deductions
            })
            .ToListAsync();

        return Ok(data);
    }

    // GET /api/salaries/{id}
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<SalaryResponseDto>> GetById(Guid id)
    {
        var s = await _db.SalaryRecords
            .AsNoTracking()
            .Include(x => x.Employee).ThenInclude(e => e.Department)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (s is null) return NotFound(new { error = "Salary record not found." });

        return Ok(new SalaryResponseDto
        {
            Id = s.Id,
            EmployeeId = s.EmployeeId,
            EmployeeName = s.Employee.FirstName + " " + s.Employee.LastName,
            EmployeeCode = s.Employee.EmployeeCode,
            DepartmentName = s.Employee.Department.Name,
            Year = s.Year,
            Month = s.Month,
            Basic = s.Basic,
            Allowances = s.Allowances,
            Deductions = s.Deductions,
            NetPay = s.Basic + s.Allowances - s.Deductions
        });
    }

    // POST /api/salaries
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSalaryDto dto)
    {
        // Validate employee
        var employeeExists = await _db.Employees.AnyAsync(e => e.Id == dto.EmployeeId);
        if (!employeeExists) return BadRequest(new { error = "Invalid EmployeeId." });

        // Validate year/month
        if (dto.Year < 2000 || dto.Year > 2100) return BadRequest(new { error = "Invalid Year." });
        if (dto.Month < 1 || dto.Month > 12) return BadRequest(new { error = "Month must be between 1 and 12." });

        // Validate amounts
        if (dto.Basic < 0 || dto.Allowances < 0 || dto.Deductions < 0)
            return BadRequest(new { error = "Amounts cannot be negative." });

        // Unique check
        var exists = await _db.SalaryRecords.AnyAsync(s =>
            s.EmployeeId == dto.EmployeeId && s.Year == dto.Year && s.Month == dto.Month);

        if (exists) return Conflict(new { error = "Salary record already exists for this employee and month." });

        var record = new SalaryRecord
        {
            EmployeeId = dto.EmployeeId,
            Year = dto.Year,
            Month = dto.Month,
            Basic = dto.Basic,
            Allowances = dto.Allowances,
            Deductions = dto.Deductions
        };

        _db.SalaryRecords.Add(record);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = record.Id }, new { record.Id });
    }

    // PUT /api/salaries/{id}
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSalaryDto dto)
    {
        if (dto.Basic < 0 || dto.Allowances < 0 || dto.Deductions < 0)
            return BadRequest(new { error = "Amounts cannot be negative." });

        var record = await _db.SalaryRecords.FirstOrDefaultAsync(s => s.Id == id);
        if (record is null) return NotFound(new { error = "Salary record not found." });

        record.Basic = dto.Basic;
        record.Allowances = dto.Allowances;
        record.Deductions = dto.Deductions;
        record.UpdatedAtUtc = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    // DELETE /api/salaries/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var record = await _db.SalaryRecords.FirstOrDefaultAsync(s => s.Id == id);
        if (record is null) return NotFound(new { error = "Salary record not found." });

        _db.SalaryRecords.Remove(record);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}