using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using StandardInteriors.Api.Dtos.Settings;
using StandardInteriors.Api.Services.Implementations;

namespace StandardInteriors.Api.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/[controller]")]
public sealed class SettingsController : ControllerBase
{
    private readonly ISettingsService _settingsService;

    public SettingsController(ISettingsService settingsService) => _settingsService = settingsService;

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken) => Ok(await _settingsService.GetAsync(cancellationToken));

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] CompanySettingsDto setting, CancellationToken cancellationToken)
        => Ok(await _settingsService.SaveAsync(setting, cancellationToken));
}
