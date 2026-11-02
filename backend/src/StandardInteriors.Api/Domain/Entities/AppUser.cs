using StandardInteriors.Api.Domain.Enums;

namespace StandardInteriors.Api.Domain.Entities;

public sealed class AppUser : AuditableEntity
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public bool CanVoidPayments { get; set; }
    public bool IsActive { get; set; } = true;
}
