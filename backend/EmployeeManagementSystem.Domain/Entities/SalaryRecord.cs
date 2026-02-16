using EmployeeManagementSystem.Domain.Common;

namespace EmployeeManagementSystem.Domain.Entities;

public class SalaryRecord : BaseEntity
{
    public Guid EmployeeId { get; set; }
    public Employee Employee { get; set; } = default!;

    public int Year { get; set; }
    public int Month { get; set; } // 1-12

    public decimal Basic { get; set; }
    public decimal Allowances { get; set; }
    public decimal Deductions { get; set; }

    public decimal NetPay => Basic + Allowances - Deductions;
}