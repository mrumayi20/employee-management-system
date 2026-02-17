namespace EmployeeManagementSystem.Application.Salary;

public class CreateSalaryDto
{
    public Guid EmployeeId { get; set; }
    public int Year { get; set; }
    public int Month { get; set; } // 1-12

    public decimal Basic { get; set; }
    public decimal Allowances { get; set; }
    public decimal Deductions { get; set; }
}