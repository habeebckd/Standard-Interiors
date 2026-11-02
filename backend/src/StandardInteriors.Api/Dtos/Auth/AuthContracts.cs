using StandardInteriors.Api.Domain.Enums;

namespace StandardInteriors.Api.Dtos.Auth;

public sealed record LoginRequest(string Username, string Password);

public sealed record UserDto(int Id, string Username, string DisplayName, UserRole Role, bool CanVoidPayments, bool IsActive);

public sealed record LoginResponse(string Token, DateTime ExpiresAt, UserDto User);

public sealed record UserUpsertDto
{
    public string Username { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public string? Password { get; init; }
    public UserRole Role { get; init; } = UserRole.Staff;
    public bool CanVoidPayments { get; init; }
    public bool IsActive { get; init; } = true;
}
