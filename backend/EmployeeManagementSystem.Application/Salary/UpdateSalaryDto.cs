namespace EmployeeManagementSystem.Application.Salary;

public class UpdateSalaryDto
{
    public decimal Basic { get; set; }
    public decimal Allowances { get; set; }
    public decimal Deductions { get; set; }
}