using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using StandardInteriors.Api.Data;
using StandardInteriors.Api.Domain.Entities;
using StandardInteriors.Api.Domain.Enums;
using StandardInteriors.Api.Dtos.Auth;
using StandardInteriors.Api.Options;

namespace StandardInteriors.Api.Services.Implementations;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<UserDto?> GetCurrentUserAsync(int userId, CancellationToken cancellationToken = default);
}

public interface IUserService
{
    Task<IReadOnlyList<UserDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<int> CreateAsync(UserUpsertDto dto, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(int id, UserUpsertDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeactivateAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ActivateAsync(int id, CancellationToken cancellationToken = default);
}

public interface ICurrentUserService
{
    int? UserId { get; }
    string? Username { get; }
}

public sealed class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor) => _httpContextAccessor = httpContextAccessor;

    public int? UserId
    {
        get
        {
            var value = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(value, out var id) ? id : null;
        }
    }

    public string? Username => _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Name);
}

public sealed class AuthService : IAuthService
{
    private readonly StandardInteriorsDbContext _db;
    private readonly IPasswordHasher<AppUser> _passwordHasher;
    private readonly JwtOptions _options;

    public AuthService(StandardInteriorsDbContext db, IPasswordHasher<AppUser> passwordHasher, IOptions<JwtOptions> options)
    {
        _db = db;
        _passwordHasher = passwordHasher;
        _options = options.Value;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var username = request.Username.Trim().ToLowerInvariant();
        var user = await _db.Users.FirstOrDefaultAsync(x => x.Username == username && x.IsActive, cancellationToken);
        if (user == null || _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password) == PasswordVerificationResult.Failed)
        {
            return null;
        }

        if (string.IsNullOrWhiteSpace(_options.Key) || Encoding.UTF8.GetByteCount(_options.Key) < 32)
        {
            throw new InvalidOperationException("JWT signing key is not configured. Set STANDARD_INTERIORS_JWT_KEY to a 32-byte-or-longer secret.");
        }

        var expiresAt = DateTime.UtcNow.AddMinutes(_options.ExpirationMinutes);
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.GivenName, user.DisplayName),
            new(ClaimTypes.Role, user.Role.ToString())
        };
        if (user.CanVoidPayments || user.Role == UserRole.Admin)
        {
            claims.Add(new Claim("permission", "payments.void"));
        }

        var credentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key)), SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(_options.Issuer, _options.Audience, claims, expires: expiresAt, signingCredentials: credentials);
        return new LoginResponse(new JwtSecurityTokenHandler().WriteToken(token), expiresAt, ToDto(user));
    }

    public async Task<UserDto?> GetCurrentUserAsync(int userId, CancellationToken cancellationToken = default)
    {
        var user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        return user == null ? null : ToDto(user);
    }

    internal static UserDto ToDto(AppUser user) => new(user.Id, user.Username, user.DisplayName, user.Role, user.CanVoidPayments, user.IsActive);
}

public sealed class UserService : IUserService
{
    private readonly StandardInteriorsDbContext _db;
    private readonly IPasswordHasher<AppUser> _passwordHasher;

    public UserService(StandardInteriorsDbContext db, IPasswordHasher<AppUser> passwordHasher)
    {
        _db = db;
        _passwordHasher = passwordHasher;
    }

    public async Task<IReadOnlyList<UserDto>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _db.Users.AsNoTracking().OrderBy(x => x.Username).Select(x => new UserDto(x.Id, x.Username, x.DisplayName, x.Role, x.CanVoidPayments, x.IsActive)).ToListAsync(cancellationToken);

    public async Task<int> CreateAsync(UserUpsertDto dto, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(dto.Password) || dto.Password.Length < 12) throw new ArgumentException("A password of at least 12 characters is required.");
        var username = dto.Username.Trim().ToLowerInvariant();
        if (await _db.Users.AnyAsync(x => x.Username == username, cancellationToken)) throw new InvalidOperationException("Username already exists.");

        var user = new AppUser { Username = username, DisplayName = dto.DisplayName.Trim(), Role = dto.Role, CanVoidPayments = dto.CanVoidPayments, IsActive = dto.IsActive };
        user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);
        _db.Users.Add(user);
        await _db.SaveChangesAsync(cancellationToken);
        return user.Id;
    }

    public async Task<bool> UpdateAsync(int id, UserUpsertDto dto, CancellationToken cancellationToken = default)
    {
        var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (user == null) return false;
        user.DisplayName = dto.DisplayName.Trim();
        user.Role = dto.Role;
        user.CanVoidPayments = dto.CanVoidPayments;
        user.IsActive = dto.IsActive;
        if (!string.IsNullOrWhiteSpace(dto.Password))
        {
            if (dto.Password.Length < 12) throw new ArgumentException("A password of at least 12 characters is required.");
            user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);
        }
        user.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeactivateAsync(int id, CancellationToken cancellationToken = default)
    {
        var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (user == null) return false;
        user.IsActive = false;
        user.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> ActivateAsync(int id, CancellationToken cancellationToken = default)
    {
        var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (user == null) return false;
        user.IsActive = true;
        user.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}
