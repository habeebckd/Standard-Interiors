using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using StandardInteriors.Api.Dtos.Common;
using StandardInteriors.Api.Dtos.Customers;
using StandardInteriors.Api.Dtos.Estimates;
using StandardInteriors.Api.Dtos.Payments;
using StandardInteriors.Api.Services.Implementations;

namespace StandardInteriors.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public sealed class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;
    private readonly IDocumentService _documentService;
    private readonly IEstimateService _estimateService;
    private readonly IPaymentService _paymentService;
    private readonly IWorkStatusService _workStatusService;

    public CustomersController(ICustomerService customerService, IDocumentService documentService, IEstimateService estimateService, IPaymentService paymentService, IWorkStatusService workStatusService)
    {
        _customerService = customerService;
        _documentService = documentService;
        _estimateService = estimateService;
        _paymentService = paymentService;
        _workStatusService = workStatusService;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] PagingQuery query, [FromQuery] bool? hasOutstanding, [FromQuery] bool? fullyPaid, [FromQuery] bool? hasEstimates, [FromQuery] DateTime? from, [FromQuery] DateTime? to, CancellationToken cancellationToken)
        => Ok(await _customerService.GetCustomersAsync(query, hasOutstanding, fullyPaid, hasEstimates, from, to, cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        => await _customerService.GetByIdAsync(id, cancellationToken) is { } item ? Ok(item) : NotFound();

    [HttpPost]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> Create([FromBody] CustomerUpsertDto dto, CancellationToken cancellationToken)
    {
        var id = await _customerService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> Update(int id, [FromBody] CustomerUpsertDto dto, CancellationToken cancellationToken)
        => await _customerService.UpdateAsync(id, dto, cancellationToken) ? NoContent() : NotFound();

    [HttpGet("{id:int}/summary")]
    public async Task<IActionResult> Summary(int id, CancellationToken cancellationToken)
        => await _customerService.GetSummaryAsync(id, cancellationToken) is { } summary ? Ok(summary) : NotFound();

    [HttpGet("{id:int}/estimates")]
    public async Task<IActionResult> Estimates(int id, CancellationToken cancellationToken)
        => Ok(await _estimateService.GetEstimatesAsync(new PagingQuery(1, 500), id, null, null, null, cancellationToken));

    [HttpGet("{id:int}/payments")]
    public async Task<IActionResult> Payments(int id, CancellationToken cancellationToken)
        => Ok(await _paymentService.GetPaymentsAsync(new PagingQuery(1, 500), id, null, null, null, null, cancellationToken));

    [HttpGet("{id:int}/projects")]
    public async Task<IActionResult> Projects(int id, CancellationToken cancellationToken)
        => Ok(await _workStatusService.GetProjectsAsync(id, cancellationToken));

    [HttpGet("{id:int}/statement")]
    public async Task<IActionResult> Statement(int id, CancellationToken cancellationToken)
        => await _customerService.GetStatementAsync(id, cancellationToken) is { } statement ? Ok(statement) : NotFound();

    [HttpGet("{id:int}/summary-pdf")]
    public async Task<IActionResult> SummaryPdf(int id, CancellationToken cancellationToken)
    {
        var customer = await _customerService.GetByIdAsync(id, cancellationToken);
        if (customer == null) return NotFound();
        var bytes = await _documentService.GenerateCustomerSummaryPdfAsync(id, cancellationToken);
        return File(bytes, "application/pdf", $"{customer.CustomerCode}-summary.pdf");
    }

    [HttpGet("{id:int}/statement-pdf")]
    public async Task<IActionResult> StatementPdf(int id, CancellationToken cancellationToken)
    {
        var customer = await _customerService.GetByIdAsync(id, cancellationToken);
        if (customer == null) return NotFound();
        var bytes = await _documentService.GenerateCustomerStatementPdfAsync(id, cancellationToken);
        return File(bytes, "application/pdf", $"{customer.CustomerCode}-statement.pdf");
    }
}
