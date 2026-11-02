using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using StandardInteriors.Api.Dtos.Services;
using StandardInteriors.Api.Services.Implementations;

namespace StandardInteriors.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public sealed class ServicesController : ControllerBase
{
    private readonly IServiceCatalogService _serviceCatalogService;

    public ServicesController(IServiceCatalogService serviceCatalogService) => _serviceCatalogService = serviceCatalogService;

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken) => Ok(await _serviceCatalogService.GetAllAsync(cancellationToken));

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] ServiceUpsertDto dto, CancellationToken cancellationToken)
    {
        var id = await _serviceCatalogService.CreateAsync(dto, cancellationToken);
        return Ok(new { id });
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] ServiceUpsertDto dto, CancellationToken cancellationToken)
        => await _serviceCatalogService.UpdateAsync(id, dto, cancellationToken) ? NoContent() : NotFound();

    [HttpPatch("{id:int}/active")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Toggle(int id, [FromQuery] bool isActive, CancellationToken cancellationToken)
        => await _serviceCatalogService.ToggleAsync(id, isActive, cancellationToken) ? NoContent() : NotFound();
}
