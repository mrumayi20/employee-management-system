using EmployeeManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Attendance> Attendances => Set<Attendance>();
    public DbSet<SalaryRecord> SalaryRecords => Set<SalaryRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Department
        modelBuilder.Entity<Department>(e =>
        {
            e.Property(x => x.Name).HasMaxLength(100).IsRequired();
            e.HasIndex(x => x.Name).IsUnique();
        });

        // Employee
        modelBuilder.Entity<Employee>(e =>
        {
            e.Property(x => x.EmployeeCode).HasMaxLength(20).IsRequired();
            e.HasIndex(x => x.EmployeeCode).IsUnique();

            e.Property(x => x.FirstName).HasMaxLength(50).IsRequired();
            e.Property(x => x.LastName).HasMaxLength(50).IsRequired();

            e.Property(x => x.Email).HasMaxLength(120).IsRequired();
            e.HasIndex(x => x.Email).IsUnique();

            e.Property(x => x.Phone).HasMaxLength(20).IsRequired();

            e.HasOne(x => x.Department)
             .WithMany(d => d.Employees)
             .HasForeignKey(x => x.DepartmentId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        // Attendance: unique per employee per date
        modelBuilder.Entity<Attendance>(e =>
        {
            e.HasIndex(x => new { x.EmployeeId, x.Date }).IsUnique();
        });

        // SalaryRecord: unique per employee per year+month
        modelBuilder.Entity<SalaryRecord>(e =>
        {
            e.HasIndex(x => new { x.EmployeeId, x.Year, x.Month }).IsUnique();
        });
    }
}