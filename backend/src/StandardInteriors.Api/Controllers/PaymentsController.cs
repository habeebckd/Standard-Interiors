using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using StandardInteriors.Api.Dtos.Common;
using StandardInteriors.Api.Dtos.Payments;
using StandardInteriors.Api.Services.Implementations;

namespace StandardInteriors.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public sealed class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly IDocumentService _documentService;

    public PaymentsController(IPaymentService paymentService, IDocumentService documentService)
    {
        _paymentService = paymentService;
        _documentService = documentService;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] PagingQuery query, [FromQuery] int? customerId, [FromQuery] int? estimateId, [FromQuery] string? status, [FromQuery] DateTime? from, [FromQuery] DateTime? to, CancellationToken cancellationToken)
        => Ok(await _paymentService.GetPaymentsAsync(query, customerId, estimateId, Enum.TryParse<StandardInteriors.Api.Domain.Enums.PaymentStatus>(status, true, out var parsed) ? parsed : null, from, to, cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        => await _paymentService.GetByIdAsync(id, cancellationToken) is { } item ? Ok(item) : NotFound();

    [HttpPost]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> Create([FromBody] PaymentUpsertDto dto, CancellationToken cancellationToken)
    {
        var id = await _paymentService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> Update(int id, [FromBody] PaymentUpsertDto dto, CancellationToken cancellationToken)
        => await _paymentService.UpdateAsync(id, dto, cancellationToken) ? NoContent() : NotFound();

    [HttpPost("{id:int}/void")]
    [Authorize(Policy = "PaymentVoid")]
    public async Task<IActionResult> Void(int id, [FromBody] PaymentVoidDto dto, CancellationToken cancellationToken)
        => await _paymentService.VoidAsync(id, dto.Reason, cancellationToken) ? NoContent() : NotFound();

    [HttpGet("{id:int}/receipt")]
    public async Task<IActionResult> Receipt(int id, CancellationToken cancellationToken)
        => await _paymentService.GetReceiptAsync(id, cancellationToken) is { } item ? Ok(item) : NotFound();

    [HttpGet("{customerId:int}/sheet")]
    public async Task<IActionResult> Sheet(int customerId, CancellationToken cancellationToken)
        => await _paymentService.GetSheetAsync(customerId, cancellationToken) is { } item ? Ok(item) : NotFound();

    [HttpGet("{customerId:int}/sheet/pdf")]
    public async Task<IActionResult> SheetPdf(int customerId, CancellationToken cancellationToken)
    {
        var bytes = await _documentService.GeneratePaymentSheetPdfAsync(customerId, cancellationToken);
        return File(bytes, "application/pdf", $"payment-sheet-{customerId}.pdf");
    }

    [HttpGet("{id:int}/receipt/pdf")]
    public async Task<IActionResult> ReceiptPdf(int id, CancellationToken cancellationToken)
    {
        var bytes = await _documentService.GeneratePaymentReceiptPdfAsync(id, cancellationToken);
        return File(bytes, "application/pdf", $"payment-receipt-{id}.pdf");
    }
}
