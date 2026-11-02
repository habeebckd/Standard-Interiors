using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using StandardInteriors.Api.Dtos.Services;
using StandardInteriors.Api.Services.Implementations;

namespace StandardInteriors.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public sealed class PaymentMethodsController : ControllerBase
{
    private readonly IPaymentMethodService _paymentMethodService;

    public PaymentMethodsController(IPaymentMethodService paymentMethodService) => _paymentMethodService = paymentMethodService;

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken) => Ok(await _paymentMethodService.GetAllAsync(cancellationToken));

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] PaymentMethodUpsertDto dto, [FromQuery] string code, CancellationToken cancellationToken)
    {
        var id = await _paymentMethodService.CreateAsync(dto, code, cancellationToken);
        return Ok(new { id });
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] PaymentMethodUpsertDto dto, CancellationToken cancellationToken)
        => await _paymentMethodService.UpdateAsync(id, dto, cancellationToken) ? NoContent() : NotFound();

    [HttpPatch("{id:int}/active")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Toggle(int id, [FromQuery] bool isActive, CancellationToken cancellationToken)
        => await _paymentMethodService.ToggleAsync(id, isActive, cancellationToken) ? NoContent() : NotFound();
}
