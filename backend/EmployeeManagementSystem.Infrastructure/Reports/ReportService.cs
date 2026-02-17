using ClosedXML.Excel;
using EmployeeManagementSystem.Application.Reports;
using EmployeeManagementSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace EmployeeManagementSystem.Infrastructure.Reports;

public class ReportService : IReportService
{
    private readonly AppDbContext _db;

    public ReportService(AppDbContext db)
    {
        _db = db;
        QuestPDF.Settings.License = LicenseType.Community; // important for QuestPDF
    }

    public async Task<byte[]> GenerateEmployeeDirectoryExcelAsync()
    {
        var employees = await _db.Employees.AsNoTracking()
            .Include(e => e.Department)
            .OrderBy(e => e.EmployeeCode)
            .Select(e => new
            {
                e.EmployeeCode,
                FullName = e.FirstName + " " + e.LastName,
                e.Email,
                e.Phone,
                Department = e.Department.Name,
                e.DateOfJoining,
                e.IsActive
            })
            .ToListAsync();

        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("Employees");

        // Header
        ws.Cell(1, 1).Value = "EmployeeCode";
        ws.Cell(1, 2).Value = "FullName";
        ws.Cell(1, 3).Value = "Email";
        ws.Cell(1, 4).Value = "Phone";
        ws.Cell(1, 5).Value = "Department";
        ws.Cell(1, 6).Value = "DateOfJoining";
        ws.Cell(1, 7).Value = "IsActive";

        ws.Range(1, 1, 1, 7).Style.Font.Bold = true;

        // Rows
        var r = 2;
        foreach (var e in employees)
        {
            ws.Cell(r, 1).Value = e.EmployeeCode;
            ws.Cell(r, 2).Value = e.FullName;
            ws.Cell(r, 3).Value = e.Email;
            ws.Cell(r, 4).Value = e.Phone;
            ws.Cell(r, 5).Value = e.Department;
            ws.Cell(r, 6).Value = e.DateOfJoining.ToString("yyyy-MM-dd");
            ws.Cell(r, 7).Value = e.IsActive ? "Yes" : "No";
            r++;
        }

        ws.Columns().AdjustToContents();

        using var ms = new MemoryStream();
        wb.SaveAs(ms);
        return ms.ToArray();
    }

    public async Task<byte[]> GenerateEmployeeDirectoryPdfAsync()
    {
        var employees = await _db.Employees.AsNoTracking()
            .Include(e => e.Department)
            .OrderBy(e => e.EmployeeCode)
            .Select(e => new
            {
                e.EmployeeCode,
                FullName = e.FirstName + " " + e.LastName,
                e.Email,
                Department = e.Department.Name,
                e.IsActive
            })
            .ToListAsync();

        var doc = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(25);
                page.Size(PageSizes.A4);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header().Text("Employee Directory").SemiBold().FontSize(16);

                page.Content().Table(table =>
                {
                    table.ColumnsDefinition(cols =>
                    {
                        cols.RelativeColumn(2); // code
                        cols.RelativeColumn(3); // name
                        cols.RelativeColumn(4); // email
                        cols.RelativeColumn(3); // dept
                        cols.RelativeColumn(1); // active
                    });

                    table.Header(h =>
                    {
                        h.Cell().Element(CellStyle).Text("Code").SemiBold();
                        h.Cell().Element(CellStyle).Text("Name").SemiBold();
                        h.Cell().Element(CellStyle).Text("Email").SemiBold();
                        h.Cell().Element(CellStyle).Text("Department").SemiBold();
                        h.Cell().Element(CellStyle).Text("Active").SemiBold();
                    });

                    foreach (var e in employees)
                    {
                        table.Cell().Element(CellStyle).Text(e.EmployeeCode);
                        table.Cell().Element(CellStyle).Text(e.FullName);
                        table.Cell().Element(CellStyle).Text(e.Email);
                        table.Cell().Element(CellStyle).Text(e.Department);
                        table.Cell().Element(CellStyle).Text(e.IsActive ? "Yes" : "No");
                    }
                });

                page.Footer().AlignRight().Text(x =>
                {
                    x.Span("Generated: ");
                    x.Span(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm 'UTC'"));
                });
            });
        });

        return doc.GeneratePdf();

        static IContainer CellStyle(IContainer c) =>
            c.Border(1).BorderColor(Colors.Grey.Lighten2).Padding(4);
    }

    // Departments (simple versions)
    public async Task<byte[]> GenerateDepartmentsExcelAsync()
    {
        var departments = await _db.Departments.AsNoTracking()
            .OrderBy(d => d.Name)
            .Select(d => new { d.Name, d.Description })
            .ToListAsync();

        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("Departments");
        ws.Cell(1, 1).Value = "Name";
        ws.Cell(1, 2).Value = "Description";
        ws.Range(1, 1, 1, 2).Style.Font.Bold = true;

        var r = 2;
        foreach (var d in departments)
        {
            ws.Cell(r, 1).Value = d.Name;
            ws.Cell(r, 2).Value = d.Description ?? "";
            r++;
        }

        ws.Columns().AdjustToContents();
        using var ms = new MemoryStream();
        wb.SaveAs(ms);
        return ms.ToArray();
    }

    public async Task<byte[]> GenerateDepartmentsPdfAsync()
    {
        var departments = await _db.Departments.AsNoTracking()
            .OrderBy(d => d.Name)
            .Select(d => new { d.Name, d.Description })
            .ToListAsync();

        var doc = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(25);
                page.Size(PageSizes.A4);
                page.DefaultTextStyle(x => x.FontSize(10));
                page.Header().Text("Departments").SemiBold().FontSize(16);

                page.Content().Table(t =>
                {
                    t.ColumnsDefinition(c =>
                    {
                        c.RelativeColumn(3);
                        c.RelativeColumn(7);
                    });

                    t.Header(h =>
                    {
                        h.Cell().Element(Cell).Text("Name").SemiBold();
                        h.Cell().Element(Cell).Text("Description").SemiBold();
                    });

                    foreach (var d in departments)
                    {
                        t.Cell().Element(Cell).Text(d.Name);
                        t.Cell().Element(Cell).Text(d.Description ?? "");
                    }
                });
            });
        });

        return doc.GeneratePdf();

        static IContainer Cell(IContainer c) =>
            c.Border(1).BorderColor(Colors.Grey.Lighten2).Padding(4);
    }

    // Attendance (date range)
    public async Task<byte[]> GenerateAttendanceExcelAsync(DateOnly from, DateOnly to)
    {
        var rows = await _db.Attendances.AsNoTracking()
            .Include(a => a.Employee).ThenInclude(e => e.Department)
            .Where(a => a.Date >= from && a.Date <= to)
            .OrderByDescending(a => a.Date)
            .ThenBy(a => a.Employee.EmployeeCode)
            .Select(a => new
            {
                a.Date,
                a.Employee.EmployeeCode,
                Name = a.Employee.FirstName + " " + a.Employee.LastName,
                Department = a.Employee.Department.Name,
                Status = a.Status.ToString(),
                a.CheckIn,
                a.CheckOut
            })
            .ToListAsync();

        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("Attendance");

        ws.Cell(1, 1).Value = "Date";
        ws.Cell(1, 2).Value = "EmployeeCode";
        ws.Cell(1, 3).Value = "Name";
        ws.Cell(1, 4).Value = "Department";
        ws.Cell(1, 5).Value = "Status";
        ws.Cell(1, 6).Value = "CheckIn";
        ws.Cell(1, 7).Value = "CheckOut";

        ws.Range(1, 1, 1, 7).Style.Font.Bold = true;

        var r = 2;
        foreach (var x in rows)
        {
            ws.Cell(r, 1).Value = x.Date.ToString("yyyy-MM-dd");
            ws.Cell(r, 2).Value = x.EmployeeCode;
            ws.Cell(r, 3).Value = x.Name;
            ws.Cell(r, 4).Value = x.Department;
            ws.Cell(r, 5).Value = x.Status;
            ws.Cell(r, 6).Value = x.CheckIn?.ToString("HH:mm") ?? "";
            ws.Cell(r, 7).Value = x.CheckOut?.ToString("HH:mm") ?? "";
            r++;
        }

        ws.Columns().AdjustToContents();
        using var ms = new MemoryStream();
        wb.SaveAs(ms);
        return ms.ToArray();
    }

    public async Task<byte[]> GenerateAttendancePdfAsync(DateOnly from, DateOnly to)
    {
        var rows = await _db.Attendances.AsNoTracking()
            .Include(a => a.Employee).ThenInclude(e => e.Department)
            .Where(a => a.Date >= from && a.Date <= to)
            .OrderByDescending(a => a.Date)
            .ThenBy(a => a.Employee.EmployeeCode)
            .Select(a => new
            {
                a.Date,
                a.Employee.EmployeeCode,
                Name = a.Employee.FirstName + " " + a.Employee.LastName,
                Department = a.Employee.Department.Name,
                Status = a.Status.ToString()
            })
            .ToListAsync();

        var title = $"Attendance Report ({from:yyyy-MM-dd} to {to:yyyy-MM-dd})";

        var doc = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(25);
                page.Size(PageSizes.A4);
                page.DefaultTextStyle(x => x.FontSize(10));
                page.Header().Text(title).SemiBold().FontSize(14);

                page.Content().Table(t =>
                {
                    t.ColumnsDefinition(c =>
                    {
                        c.RelativeColumn(2); // date
                        c.RelativeColumn(2); // code
                        c.RelativeColumn(3); // name
                        c.RelativeColumn(3); // dept
                        c.RelativeColumn(2); // status
                    });

                    t.Header(h =>
                    {
                        h.Cell().Element(Cell).Text("Date").SemiBold();
                        h.Cell().Element(Cell).Text("Code").SemiBold();
                        h.Cell().Element(Cell).Text("Name").SemiBold();
                        h.Cell().Element(Cell).Text("Department").SemiBold();
                        h.Cell().Element(Cell).Text("Status").SemiBold();
                    });

                    foreach (var x in rows)
                    {
                        t.Cell().Element(Cell).Text(x.Date.ToString("yyyy-MM-dd"));
                        t.Cell().Element(Cell).Text(x.EmployeeCode);
                        t.Cell().Element(Cell).Text(x.Name);
                        t.Cell().Element(Cell).Text(x.Department);
                        t.Cell().Element(Cell).Text(x.Status);
                    }
                });
            });
        });

        return doc.GeneratePdf();

        static IContainer Cell(IContainer c) =>
            c.Border(1).BorderColor(Colors.Grey.Lighten2).Padding(4);
    }

    // Salary (year+month)
    public async Task<byte[]> GenerateSalaryExcelAsync(int year, int month)
    {
        var rows = await _db.SalaryRecords.AsNoTracking()
            .Include(s => s.Employee).ThenInclude(e => e.Department)
            .Where(s => s.Year == year && s.Month == month)
            .OrderBy(s => s.Employee.EmployeeCode)
            .Select(s => new
            {
                s.Employee.EmployeeCode,
                Name = s.Employee.FirstName + " " + s.Employee.LastName,
                Department = s.Employee.Department.Name,
                s.Basic,
                s.Allowances,
                s.Deductions,
                NetPay = s.Basic + s.Allowances - s.Deductions
            })
            .ToListAsync();

        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("Salary");

        ws.Cell(1, 1).Value = "EmployeeCode";
        ws.Cell(1, 2).Value = "Name";
        ws.Cell(1, 3).Value = "Department";
        ws.Cell(1, 4).Value = "Basic";
        ws.Cell(1, 5).Value = "Allowances";
        ws.Cell(1, 6).Value = "Deductions";
        ws.Cell(1, 7).Value = "NetPay";
        ws.Range(1, 1, 1, 7).Style.Font.Bold = true;

        var r = 2;
        foreach (var x in rows)
        {
            ws.Cell(r, 1).Value = x.EmployeeCode;
            ws.Cell(r, 2).Value = x.Name;
            ws.Cell(r, 3).Value = x.Department;
            ws.Cell(r, 4).Value = x.Basic;
            ws.Cell(r, 5).Value = x.Allowances;
            ws.Cell(r, 6).Value = x.Deductions;
            ws.Cell(r, 7).Value = x.NetPay;
            r++;
        }

        ws.Columns().AdjustToContents();
        using var ms = new MemoryStream();
        wb.SaveAs(ms);
        return ms.ToArray();
    }

    public async Task<byte[]> GenerateSalaryPdfAsync(int year, int month)
    {
        var rows = await _db.SalaryRecords.AsNoTracking()
            .Include(s => s.Employee).ThenInclude(e => e.Department)
            .Where(s => s.Year == year && s.Month == month)
            .OrderBy(s => s.Employee.EmployeeCode)
            .Select(s => new
            {
                s.Employee.EmployeeCode,
                Name = s.Employee.FirstName + " " + s.Employee.LastName,
                Department = s.Employee.Department.Name,
                NetPay = s.Basic + s.Allowances - s.Deductions
            })
            .ToListAsync();

        var title = $"Salary Report ({year}-{month:00})";

        var doc = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(25);
                page.Size(PageSizes.A4);
                page.DefaultTextStyle(x => x.FontSize(10));
                page.Header().Text(title).SemiBold().FontSize(14);

                page.Content().Table(t =>
                {
                    t.ColumnsDefinition(c =>
                    {
                        c.RelativeColumn(2);
                        c.RelativeColumn(4);
                        c.RelativeColumn(4);
                        c.RelativeColumn(2);
                    });

                    t.Header(h =>
                    {
                        h.Cell().Element(Cell).Text("Code").SemiBold();
                        h.Cell().Element(Cell).Text("Name").SemiBold();
                        h.Cell().Element(Cell).Text("Department").SemiBold();
                        h.Cell().Element(Cell).Text("NetPay").SemiBold();
                    });

                    foreach (var x in rows)
                    {
                        t.Cell().Element(Cell).Text(x.EmployeeCode);
                        t.Cell().Element(Cell).Text(x.Name);
                        t.Cell().Element(Cell).Text(x.Department);
                        t.Cell().Element(Cell).Text(x.NetPay.ToString("0.00"));
                    }
                });
            });
        });

        return doc.GeneratePdf();

        static IContainer Cell(IContainer c) =>
            c.Border(1).BorderColor(Colors.Grey.Lighten2).Padding(4);
    }
}