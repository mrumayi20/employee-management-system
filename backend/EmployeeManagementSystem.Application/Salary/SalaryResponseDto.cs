namespace EmployeeManagementSystem.Application.Salary;

public class SalaryResponseDto
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = default!;
    public string EmployeeCode { get; set; } = default!;
    public string DepartmentName { get; set; } = default!;

    public int Year { get; set; }
    public int Month { get; set; }

    public decimal Basic { get; set; }
    public decimal Allowances { get; set; }
    public decimal Deductions { get; set; }
    public decimal NetPay { get; set; }
}