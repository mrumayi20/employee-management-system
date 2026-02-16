using EmployeeManagementSystem.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet("db")]
    public async Task<IActionResult> Db([FromServices] AppDbContext db)
    {
        var canConnect = await db.Database.CanConnectAsync();
        return Ok(new { database = canConnect ? "up" : "down" });
    }
}