using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using StandardInteriors.Api.Dtos.Common;
using StandardInteriors.Api.Dtos.Estimates;
using StandardInteriors.Api.Services.Implementations;

namespace StandardInteriors.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public sealed class EstimatesController : ControllerBase
{
    private readonly IEstimateService _estimateService;
    private readonly IDocumentService _documentService;
    private readonly IWorkStatusService _workStatusService;

    public EstimatesController(IEstimateService estimateService, IDocumentService documentService, IWorkStatusService workStatusService)
    {
        _estimateService = estimateService;
        _documentService = documentService;
        _workStatusService = workStatusService;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] PagingQuery query, [FromQuery] int? customerId, [FromQuery] string? status, [FromQuery] DateTime? from, [FromQuery] DateTime? to, CancellationToken cancellationToken)
        => Ok(await _estimateService.GetEstimatesAsync(query, customerId, Enum.TryParse<StandardInteriors.Api.Domain.Enums.EstimateStatus>(status, true, out var parsed) ? parsed : null, from, to, cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        => await _estimateService.GetByIdAsync(id, cancellationToken) is { } item ? Ok(item) : NotFound();

    [HttpPost]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> Create([FromBody] EstimateUpsertDto dto, CancellationToken cancellationToken)
    {
        var id = await _estimateService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> Update(int id, [FromBody] EstimateUpsertDto dto, CancellationToken cancellationToken)
        => await _estimateService.UpdateAsync(id, dto, cancellationToken) ? NoContent() : NotFound();

    [HttpPost("{id:int}/duplicate")]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> Duplicate(int id, CancellationToken cancellationToken)
        => await _estimateService.DuplicateAsync(id, cancellationToken) is { } newId ? Ok(new { id = newId }) : NotFound();

    [HttpPut("{id:int}/status")]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> Status(int id, [FromBody] EstimateStatusChangeDto dto, CancellationToken cancellationToken)
        => await _estimateService.ChangeStatusAsync(id, dto, cancellationToken) ? NoContent() : NotFound();

    [HttpGet("{id:int}/work-status")]
    public async Task<IActionResult> WorkStatus(int id, CancellationToken cancellationToken)
        => await _workStatusService.GetAsync(id, cancellationToken) is { } item ? Ok(item) : NotFound();

    [HttpGet("{id:int}/work-status-history")]
    public async Task<IActionResult> WorkStatusHistory(int id, CancellationToken cancellationToken)
        => Ok(await _workStatusService.GetHistoryAsync(id, cancellationToken));

    [HttpPut("{id:int}/work-status")]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> UpdateWorkStatus(int id, [FromBody] WorkStatusUpdateDto dto, CancellationToken cancellationToken)
    {
        try { return await _workStatusService.UpdateAsync(id, dto, cancellationToken) ? NoContent() : NotFound(); }
        catch (ArgumentException exception) { return BadRequest(new { detail = exception.Message }); }
    }

    [HttpPost("{id:int}/start-work")]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> StartWork(int id, [FromBody] WorkStatusUpdateDto dto, CancellationToken cancellationToken)
    {
        try { return await _workStatusService.StartAsync(id, dto, cancellationToken) ? NoContent() : NotFound(); }
        catch (ArgumentException exception) { return BadRequest(new { detail = exception.Message }); }
    }

    [HttpPost("{id:int}/resume-work")]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> ResumeWork(int id, [FromBody] WorkStatusUpdateDto dto, CancellationToken cancellationToken)
    {
        try { return await _workStatusService.ResumeAsync(id, dto, cancellationToken) ? NoContent() : NotFound(); }
        catch (ArgumentException exception) { return BadRequest(new { detail = exception.Message }); }
    }

    [HttpGet("{id:int}/pdf")]
    public async Task<IActionResult> Pdf(int id, CancellationToken cancellationToken)
    {
        var bytes = await _documentService.GenerateEstimatePdfAsync(id, cancellationToken);
        return File(bytes, "application/pdf", $"estimate-{id}.pdf");
    }
}
