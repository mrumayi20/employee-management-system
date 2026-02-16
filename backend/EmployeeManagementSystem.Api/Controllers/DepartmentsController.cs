using EmployeeManagementSystem.Domain.Entities;
using EmployeeManagementSystem.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DepartmentsController : ControllerBase
{
    private readonly AppDbContext _db;

    public DepartmentsController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var depts = await _db.Departments
            .AsNoTracking()
            .OrderBy(d => d.Name)
            .Select(d => new { d.Id, d.Name, d.Description })
            .ToListAsync();

        return Ok(depts);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDepartmentRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return BadRequest(new { error = "Department name is required." });

        var name = request.Name.Trim();

        var exists = await _db.Departments.AnyAsync(d => d.Name == name);
        if (exists)
            return Conflict(new { error = "Department already exists." });

        var dept = new Department
        {
            Name = name,
            Description = request.Description?.Trim()
        };

        _db.Departments.Add(dept);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAll), new { id = dept.Id }, new { dept.Id, dept.Name, dept.Description });
    }
}

public record CreateDepartmentRequest(string Name, string? Description);