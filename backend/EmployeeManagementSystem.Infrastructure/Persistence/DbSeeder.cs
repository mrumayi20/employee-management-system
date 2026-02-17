using EmployeeManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.Infrastructure.Persistence;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        await db.Database.MigrateAsync();

        // Seed Departments
        if (!await db.Departments.AnyAsync())
        {
            db.Departments.AddRange(
                new Department { Name = "Engineering", Description = "Builds products" },
                new Department { Name = "HR", Description = "People operations" },
                new Department { Name = "Finance", Description = "Billing and payroll" }
            );
        }

        // Seed Admin user
        var adminEmail = "admin@ems.com";
        if (!await db.Users.AnyAsync(u => u.Email == adminEmail))
        {
            db.Users.Add(new User
            {
                FullName = "Admin User",
                Email = adminEmail,
                Role = "Admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                IsActive = true
            });
        }

        await db.SaveChangesAsync();
    }
}