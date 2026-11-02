using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using StandardInteriors.Api.Domain.Entities;
using StandardInteriors.Api.Domain.Enums;

namespace StandardInteriors.Api.Data;

public static class DbSeeder
{
    public static async Task EnsureSeededAsync(StandardInteriorsDbContext db, CancellationToken cancellationToken = default)
    {
        if (!await db.CompanySettings.AnyAsync(cancellationToken))
        {
            db.CompanySettings.Add(new CompanySetting
            {
                CompanyName = "STANDARD INTERIORS",
                EstimatePrefix = "EST",
                PaymentPrefix = "PAY",
                Currency = "INR",
                DefaultEstimateNote = "This estimate is prepared for the listed interior works. Final cost may vary based on site measurements, material/brand selection, design changes and additional work approved during execution.",
                LogoPath = "..\\..\\..\\..\\..\\..\\assets\\logo\\standard-interiors-logo.png"
            });

            await db.SaveChangesAsync(cancellationToken);
        }
    }

    public static async Task EnsureAdminAsync(StandardInteriorsDbContext db, IPasswordHasher<AppUser> passwordHasher, IConfiguration configuration, ILogger logger, CancellationToken cancellationToken = default)
    {
        if (await db.Users.AnyAsync(cancellationToken)) return;

        var password = Environment.GetEnvironmentVariable("STANDARD_INTERIORS_ADMIN_PASSWORD");
        if (string.IsNullOrWhiteSpace(password) || password.Length < 12)
        {
            throw new InvalidOperationException("Set STANDARD_INTERIORS_ADMIN_PASSWORD to a password of at least 12 characters before first startup.");
        }

        var username = (Environment.GetEnvironmentVariable("STANDARD_INTERIORS_ADMIN_USERNAME") ?? "admin").Trim().ToLowerInvariant();
        var user = new AppUser { Username = username, DisplayName = "Standard Interiors Admin", Role = UserRole.Admin, CanVoidPayments = true, IsActive = true };
        user.PasswordHash = passwordHasher.HashPassword(user, password);
        db.Users.Add(user);
        await db.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Seeded initial admin user {Username}. The password was read only from the environment.", username);
    }
}
