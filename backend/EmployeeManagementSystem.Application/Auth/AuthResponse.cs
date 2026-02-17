namespace EmployeeManagementSystem.Application.Auth;

public class AuthResponse
{
    public string Token { get; set; } = default!;
    public DateTime ExpiresAtUtc { get; set; }
}