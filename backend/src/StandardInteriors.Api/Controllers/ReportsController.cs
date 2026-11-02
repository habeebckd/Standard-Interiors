using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using StandardInteriors.Api.Domain.Enums;
using StandardInteriors.Api.Dtos.Reports;
using StandardInteriors.Api.Services.Implementations;

namespace StandardInteriors.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public sealed class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService) => _reportService = reportService;

    [HttpGet("customers")]
    public async Task<IActionResult> Customers([FromQuery] ReportFilter filter, CancellationToken cancellationToken) => Ok(await _reportService.GetCustomersAsync(filter, cancellationToken));

    [HttpGet("estimates")]
    public async Task<IActionResult> Estimates([FromQuery] ReportFilter filter, CancellationToken cancellationToken) => Ok(await _reportService.GetEstimatesAsync(filter, cancellationToken));

    [HttpGet("payments")]
    public async Task<IActionResult> Payments([FromQuery] ReportFilter filter, CancellationToken cancellationToken) => Ok(await _reportService.GetPaymentsAsync(filter, cancellationToken));

    [HttpGet("outstanding")]
    public async Task<IActionResult> Outstanding([FromQuery] ReportFilter filter, CancellationToken cancellationToken) => Ok(await _reportService.GetOutstandingAsync(filter, cancellationToken));

    [HttpGet("monthly-collections")]
    public async Task<IActionResult> MonthlyCollections([FromQuery] ReportFilter filter, CancellationToken cancellationToken) => Ok(await _reportService.GetMonthlyCollectionsAsync(filter, cancellationToken));
}
