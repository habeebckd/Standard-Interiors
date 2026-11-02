using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using StandardInteriors.Api.Domain.Enums;
using StandardInteriors.Api.Dtos.Reports;
using StandardInteriors.Api.Services.Implementations;

namespace StandardInteriors.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/exports")]
public sealed class ExportsController : ControllerBase
{
    private readonly IExportService _exports;

    public ExportsController(IExportService exports) => _exports = exports;

    [HttpGet("customers.xlsx")]
    public async Task<IActionResult> Customers([FromQuery] ReportFilter filter, CancellationToken cancellationToken)
        => File(await _exports.CustomersAsync(filter, cancellationToken), ExcelContentType, FileName("Customers"));

    [HttpGet("customers/{customerId:int}/summary.xlsx")]
    public async Task<IActionResult> CustomerSummary(int customerId, CancellationToken cancellationToken)
        => File(await _exports.CustomerSummaryAsync(customerId, cancellationToken), ExcelContentType, FileName("CustomerSummary"));

    [HttpGet("customers/{customerId:int}/statement.xlsx")]
    public async Task<IActionResult> CustomerStatement(int customerId, CancellationToken cancellationToken)
        => File(await _exports.CustomerStatementAsync(customerId, cancellationToken), ExcelContentType, FileName("CustomerStatement"));

    [HttpGet("estimates.xlsx")]
    public async Task<IActionResult> Estimates([FromQuery] ReportFilter filter, CancellationToken cancellationToken)
        => File(await _exports.EstimatesAsync(filter, cancellationToken), ExcelContentType, FileName("Estimates"));

    [HttpGet("payments.xlsx")]
    public async Task<IActionResult> Payments([FromQuery] ReportFilter filter, CancellationToken cancellationToken)
        => File(await _exports.PaymentsAsync(filter, cancellationToken), ExcelContentType, FileName("Payments"));

    [HttpGet("outstanding.xlsx")]
    public async Task<IActionResult> Outstanding([FromQuery] ReportFilter filter, CancellationToken cancellationToken)
        => File(await _exports.OutstandingAsync(filter, cancellationToken), ExcelContentType, FileName("Outstanding"));

    [HttpGet("fully-paid.xlsx")]
    public async Task<IActionResult> FullyPaid([FromQuery] ReportFilter filter, CancellationToken cancellationToken)
        => File(await _exports.FullyPaidAsync(filter, cancellationToken), ExcelContentType, FileName("FullyPaid"));

    [HttpGet("monthly-collections.xlsx")]
    public async Task<IActionResult> MonthlyCollections([FromQuery] ReportFilter filter, CancellationToken cancellationToken)
        => File(await _exports.MonthlyCollectionsAsync(filter, cancellationToken), ExcelContentType, FileName("MonthlyCollections"));

    private static string FileName(string report) => $"StandardInteriors_{report}_{DateTime.Now:yyyy-MM-dd}.xlsx";
    private const string ExcelContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
}
