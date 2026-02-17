using EmployeeManagementSystem.Application.Reports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reports;

    public ReportsController(IReportService reports) => _reports = reports;

    [HttpGet("employees/excel")]
    public async Task<IActionResult> EmployeeExcel()
    {
        var bytes = await _reports.GenerateEmployeeDirectoryExcelAsync();
        return File(bytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"employee-directory-{DateTime.UtcNow:yyyyMMddHHmm}.xlsx");
    }

    [HttpGet("employees/pdf")]
    public async Task<IActionResult> EmployeePdf()
    {
        var bytes = await _reports.GenerateEmployeeDirectoryPdfAsync();
        return File(bytes, "application/pdf",
            $"employee-directory-{DateTime.UtcNow:yyyyMMddHHmm}.pdf");
    }

    [HttpGet("departments/excel")]
    public async Task<IActionResult> DepartmentsExcel()
    {
        var bytes = await _reports.GenerateDepartmentsExcelAsync();
        return File(bytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"departments-{DateTime.UtcNow:yyyyMMddHHmm}.xlsx");
    }

    [HttpGet("departments/pdf")]
    public async Task<IActionResult> DepartmentsPdf()
    {
        var bytes = await _reports.GenerateDepartmentsPdfAsync();
        return File(bytes, "application/pdf",
            $"departments-{DateTime.UtcNow:yyyyMMddHHmm}.pdf");
    }

    // /api/reports/attendance/excel?from=2026-02-01&to=2026-02-16
    [HttpGet("attendance/excel")]
    public async Task<IActionResult> AttendanceExcel([FromQuery] DateOnly from, [FromQuery] DateOnly to)
    {
        var bytes = await _reports.GenerateAttendanceExcelAsync(from, to);
        return File(bytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"attendance-{from:yyyyMMdd}-{to:yyyyMMdd}.xlsx");
    }

    [HttpGet("attendance/pdf")]
    public async Task<IActionResult> AttendancePdf([FromQuery] DateOnly from, [FromQuery] DateOnly to)
    {
        var bytes = await _reports.GenerateAttendancePdfAsync(from, to);
        return File(bytes, "application/pdf",
            $"attendance-{from:yyyyMMdd}-{to:yyyyMMdd}.pdf");
    }

    // /api/reports/salary/excel?year=2026&month=2
    [HttpGet("salary/excel")]
    public async Task<IActionResult> SalaryExcel([FromQuery] int year, [FromQuery] int month)
    {
        var bytes = await _reports.GenerateSalaryExcelAsync(year, month);
        return File(bytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"salary-{year}-{month:00}.xlsx");
    }

    [HttpGet("salary/pdf")]
    public async Task<IActionResult> SalaryPdf([FromQuery] int year, [FromQuery] int month)
    {
        var bytes = await _reports.GenerateSalaryPdfAsync(year, month);
        return File(bytes, "application/pdf",
            $"salary-{year}-{month:00}.pdf");
    }
}