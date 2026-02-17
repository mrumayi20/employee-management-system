namespace EmployeeManagementSystem.Application.Reports;

public interface IReportService
{
    Task<byte[]> GenerateEmployeeDirectoryExcelAsync();
    Task<byte[]> GenerateEmployeeDirectoryPdfAsync();

    Task<byte[]> GenerateDepartmentsExcelAsync();
    Task<byte[]> GenerateDepartmentsPdfAsync();

    Task<byte[]> GenerateAttendanceExcelAsync(DateOnly from, DateOnly to);
    Task<byte[]> GenerateAttendancePdfAsync(DateOnly from, DateOnly to);

    Task<byte[]> GenerateSalaryExcelAsync(int year, int month);
    Task<byte[]> GenerateSalaryPdfAsync(int year, int month);
}