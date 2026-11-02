using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StandardInteriors.Api.Dtos.Auth;
using StandardInteriors.Api.Services.Implementations;

namespace StandardInteriors.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService) => _authService = authService;

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.LoginAsync(request, cancellationToken);
        return result == null ? Unauthorized(new { error = "Invalid username or password." }) : Ok(result);
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me(CancellationToken cancellationToken)
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return !int.TryParse(claim, out var userId) || await _authService.GetCurrentUserAsync(userId, cancellationToken) is not { } user ? Unauthorized() : Ok(user);
    }
}
