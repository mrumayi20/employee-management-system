using EmployeeManagementSystem.Application.DTOs;
using EmployeeManagementSystem.Domain.Entities;
using EmployeeManagementSystem.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace EmployeeManagementSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EmployeesController : ControllerBase
{
    private readonly AppDbContext _db;

    public EmployeesController(AppDbContext db) => _db = db;

    // GET: /api/employees
    [HttpGet]
    public async Task<ActionResult<List<EmployeeResponseDto>>> GetAll()
    {
        var employees = await _db.Employees
            .AsNoTracking()
            .Include(e => e.Department)
            .OrderBy(e => e.FirstName)
            .ThenBy(e => e.LastName)
            .Select(e => new EmployeeResponseDto
            {
                Id = e.Id,
                EmployeeCode = e.EmployeeCode,
                FullName = e.FirstName + " " + e.LastName,
                Email = e.Email,
                Phone = e.Phone,
                DepartmentName = e.Department.Name
            })
            .ToListAsync();

        return Ok(employees);
    }

    // GET: /api/employees/{id}
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<EmployeeResponseDto>> GetById(Guid id)
    {
        var employee = await _db.Employees
            .AsNoTracking()
            .Include(e => e.Department)
            .Where(e => e.Id == id)
            .Select(e => new EmployeeResponseDto
            {
                Id = e.Id,
                EmployeeCode = e.EmployeeCode,
                FullName = e.FirstName + " " + e.LastName,
                Email = e.Email,
                Phone = e.Phone,
                DepartmentName = e.Department.Name
            })
            .FirstOrDefaultAsync();

        if (employee is null)
            return NotFound(new { error = "Employee not found." });

        return Ok(employee);
    }

    // POST: /api/employees
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateEmployeeDto dto)
    {
        // Basic validation (we will improve later with FluentValidation)
        if (string.IsNullOrWhiteSpace(dto.EmployeeCode)) return BadRequest(new { error = "EmployeeCode is required." });
        if (string.IsNullOrWhiteSpace(dto.FirstName)) return BadRequest(new { error = "FirstName is required." });
        if (string.IsNullOrWhiteSpace(dto.LastName)) return BadRequest(new { error = "LastName is required." });
        if (string.IsNullOrWhiteSpace(dto.Email)) return BadRequest(new { error = "Email is required." });
        if (string.IsNullOrWhiteSpace(dto.Phone)) return BadRequest(new { error = "Phone is required." });

        var code = dto.EmployeeCode.Trim();
        var email = dto.Email.Trim().ToLowerInvariant();

        // Department must exist
        var deptExists = await _db.Departments.AnyAsync(d => d.Id == dto.DepartmentId);
        if (!deptExists)
            return BadRequest(new { error = "Invalid DepartmentId." });

        // Unique checks
        if (await _db.Employees.AnyAsync(e => e.EmployeeCode == code))
            return Conflict(new { error = "EmployeeCode already exists." });

        if (await _db.Employees.AnyAsync(e => e.Email == email))
            return Conflict(new { error = "Email already exists." });

        var employee = new Employee
        {
            EmployeeCode = code,
            FirstName = dto.FirstName.Trim(),
            LastName = dto.LastName.Trim(),
            Email = email,
            Phone = dto.Phone.Trim(),
            DateOfJoining = dto.DateOfJoining,
            DepartmentId = dto.DepartmentId,
            IsActive = true
        };

        _db.Employees.Add(employee);
        await _db.SaveChangesAsync();

        // Return created resource
        return CreatedAtAction(nameof(GetById), new { id = employee.Id }, new { employee.Id });
    }

    // PUT: /api/employees/{id}
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateEmployeeDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.FirstName)) return BadRequest(new { error = "FirstName is required." });
        if (string.IsNullOrWhiteSpace(dto.LastName)) return BadRequest(new { error = "LastName is required." });
        if (string.IsNullOrWhiteSpace(dto.Phone)) return BadRequest(new { error = "Phone is required." });

        var employee = await _db.Employees.FirstOrDefaultAsync(e => e.Id == id);
        if (employee is null)
            return NotFound(new { error = "Employee not found." });

        employee.FirstName = dto.FirstName.Trim();
        employee.LastName = dto.LastName.Trim();
        employee.Phone = dto.Phone.Trim();
        employee.IsActive = dto.IsActive;
        employee.UpdatedAtUtc = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: /api/employees/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var employee = await _db.Employees.FirstOrDefaultAsync(e => e.Id == id);
        if (employee is null)
            return NotFound(new { error = "Employee not found." });

        _db.Employees.Remove(employee);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}