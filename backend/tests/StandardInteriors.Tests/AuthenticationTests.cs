using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using StandardInteriors.Api.Controllers;
using StandardInteriors.Api.Data;
using StandardInteriors.Api.Domain.Entities;
using StandardInteriors.Api.Domain.Enums;
using StandardInteriors.Api.Dtos.Auth;
using StandardInteriors.Api.Options;
using StandardInteriors.Api.Services.Implementations;

namespace StandardInteriors.Tests;

public sealed class AuthenticationTests
{
    [Fact]
    public async Task Login_UsesHashedPasswordAndReturnsRoleClaimData()
    {
        var options = new DbContextOptionsBuilder<StandardInteriorsDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        await using var db = new StandardInteriorsDbContext(options);
        var hasher = new PasswordHasher<AppUser>();
        var user = new AppUser { Id = 1, Username = "staff", DisplayName = "Staff User", Role = UserRole.Staff };
        user.PasswordHash = hasher.HashPassword(user, "CorrectHorseBattery12");
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var service = new AuthService(db, hasher, Options.Create(new JwtOptions { Key = "a-secure-test-key-that-is-at-least-32-bytes-long", Issuer = "test", Audience = "test" }));
        var response = await service.LoginAsync(new LoginRequest("STAFF", "CorrectHorseBattery12"));

        Assert.NotNull(response);
        Assert.Equal(UserRole.Staff, response.User.Role);
        Assert.NotEqual("CorrectHorseBattery12", user.PasswordHash);
        Assert.True(response.Token.Split('.').Length == 3);
    }

    [Fact]
    public async Task Login_RejectsWrongPassword()
    {
        var options = new DbContextOptionsBuilder<StandardInteriorsDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        await using var db = new StandardInteriorsDbContext(options);
        var hasher = new PasswordHasher<AppUser>();
        var user = new AppUser { Username = "viewer", DisplayName = "Viewer", Role = UserRole.Viewer };
        user.PasswordHash = hasher.HashPassword(user, "CorrectHorseBattery12");
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var service = new AuthService(db, hasher, Options.Create(new JwtOptions { Key = "a-secure-test-key-that-is-at-least-32-bytes-long" }));

        Assert.Null(await service.LoginAsync(new LoginRequest("viewer", "wrong-password")));
    }

    [Fact]
    public async Task PaymentVoidHandler_AllowsPermissionAndRejectsViewer()
    {
        var handler = new PaymentVoidHandler();
        var requirement = new PaymentVoidRequirement();
        var staff = new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.Role, "Staff"), new Claim("permission", "payments.void")], "test"));
        var viewer = new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.Role, "Viewer")], "test"));

        var allowed = new AuthorizationHandlerContext([requirement], staff, null);
        await handler.HandleAsync(allowed);
        var denied = new AuthorizationHandlerContext([requirement], viewer, null);
        await handler.HandleAsync(denied);

        Assert.True(allowed.HasSucceeded);
        Assert.False(denied.HasSucceeded);
    }
}
