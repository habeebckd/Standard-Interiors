using System.Globalization;
using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using StandardInteriors.Api.Data;
using StandardInteriors.Api.Domain.Entities;
using StandardInteriors.Api.Domain.Enums;
using StandardInteriors.Api.Dtos.Customers;
using StandardInteriors.Api.Dtos.Reports;

namespace StandardInteriors.Api.Services.Implementations;

public interface IExportService
{
    Task<byte[]> CustomersAsync(ReportFilter filter, CancellationToken cancellationToken = default);
    Task<byte[]> CustomerSummaryAsync(int customerId, CancellationToken cancellationToken = default);
    Task<byte[]> CustomerStatementAsync(int customerId, CancellationToken cancellationToken = default);
    Task<byte[]> EstimatesAsync(ReportFilter filter, CancellationToken cancellationToken = default);
    Task<byte[]> PaymentsAsync(ReportFilter filter, CancellationToken cancellationToken = default);
    Task<byte[]> OutstandingAsync(ReportFilter filter, CancellationToken cancellationToken = default);
    Task<byte[]> FullyPaidAsync(ReportFilter filter, CancellationToken cancellationToken = default);
    Task<byte[]> MonthlyCollectionsAsync(ReportFilter filter, CancellationToken cancellationToken = default);
}

public sealed class ExportService : IExportService
{
    private static readonly EstimateStatus[] ContributingStatuses = [EstimateStatus.Sent, EstimateStatus.Approved, EstimateStatus.Completed];
    private readonly StandardInteriorsDbContext _db;
    private readonly ICustomerService _customerService;

    public ExportService(StandardInteriorsDbContext db, ICustomerService customerService)
    {
        _db = db;
        _customerService = customerService;
    }

    public async Task<byte[]> CustomersAsync(ReportFilter filter, CancellationToken cancellationToken = default)
    {
        var query = _db.Customers.AsNoTracking().AsQueryable();
        query = ApplyCustomerSearch(query, filter);

        var projected = query.OrderBy(x => x.Name).Select(c => new
        {
            c.CustomerCode,
            Customer = c.Name,
            c.Phone,
            c.Location,
            TotalEstimates = c.Estimates.Count(e => ContributingStatuses.Contains(e.Status)),
            EstimatedValue = c.Estimates.Where(e => ContributingStatuses.Contains(e.Status)).Sum(e => (decimal?)e.CalculatedTotalAmount) ?? 0m,
            Received = c.Payments.Where(p => p.Status == PaymentStatus.Posted).Sum(p => (decimal?)p.Amount) ?? 0m
        });

        if (filter.HasOutstanding == true) projected = projected.Where(x => x.EstimatedValue > x.Received);
        if (filter.FullyPaid == true) projected = projected.Where(x => x.TotalEstimates > 0 && x.EstimatedValue <= x.Received);

        var rows = await projected.ToListAsync(cancellationToken);

        var filtered = rows.Select(x => new AllCustomerReportRowDto(x.CustomerCode, x.Customer, x.Phone, x.Location, x.TotalEstimates, x.EstimatedValue, x.Received, x.EstimatedValue - x.Received));
        return Workbook("Customers", ["Customer Code", "Customer", "Phone", "Location", "Total Estimates", "Estimated Value", "Received", "Outstanding"], filtered.Select(x => new object?[] { x.CustomerCode, x.CustomerName, x.Phone, x.Location, x.TotalEstimates, x.EstimatedValue, x.Received, x.Outstanding }));
    }

    public async Task<byte[]> CustomerSummaryAsync(int customerId, CancellationToken cancellationToken = default)
    {
        var customer = await _customerService.GetSummaryAsync(customerId, cancellationToken) ?? throw new ArgumentException("Customer not found.");
        return Workbook("Customer Summary", ["Customer Code", "Customer", "Phone", "Location", "Total Estimates", "Total Payments", "Total Estimated", "Total Received", "Outstanding"],
            [new object?[] { customer.CustomerCode, customer.Name, customer.Phone, customer.Location, customer.TotalEstimates, customer.TotalPayments, customer.TotalEstimated, customer.TotalReceived, customer.Outstanding }]);
    }

    public async Task<byte[]> CustomerStatementAsync(int customerId, CancellationToken cancellationToken = default)
    {
        var statement = await _customerService.GetStatementAsync(customerId, cancellationToken) ?? throw new ArgumentException("Customer not found.");
        return Workbook("Customer Statement", ["Date", "Reference", "Type", "Debit", "Credit", "Running Balance"],
            statement.Lines.Select(x => new object?[] { x.Date, x.Reference, x.Type, x.Debit, x.Credit, x.RunningBalance }));
    }

    public async Task<byte[]> EstimatesAsync(ReportFilter filter, CancellationToken cancellationToken = default)
    {
        var query = _db.Estimates.AsNoTracking().Include(x => x.Customer).AsQueryable();
        if (filter.CustomerId.HasValue) query = query.Where(x => x.CustomerId == filter.CustomerId.Value);
        if (!string.IsNullOrWhiteSpace(filter.Customer)) query = query.Where(x => x.Customer.Name.Contains(filter.Customer));
        if (filter.EstimateStatus.HasValue) query = query.Where(x => x.Status == filter.EstimateStatus.Value);
        if (filter.DateFrom.HasValue) query = query.Where(x => x.EstimateDate >= filter.DateFrom.Value);
        if (filter.DateTo.HasValue) query = query.Where(x => x.EstimateDate <= filter.DateTo.Value);

        var rows = await query.OrderByDescending(x => x.EstimateDate).Select(x => new
        {
            x.EstimateNumber,
            Customer = x.Customer.Name,
            x.EstimateDate,
            Status = x.Status,
            x.CalculatedTotalAmount,
            x.CalculatedReceivedAmount,
            x.CalculatedBalance
        }).ToListAsync(cancellationToken);

        return Workbook("Estimates", ["Estimate No", "Customer", "Estimate Date", "Status", "Total Amount", "Received", "Outstanding"], rows.Select(x => new object?[] { x.EstimateNumber, x.Customer, x.EstimateDate, x.Status.ToString(), x.CalculatedTotalAmount, x.CalculatedReceivedAmount, x.CalculatedBalance }));
    }

    public async Task<byte[]> PaymentsAsync(ReportFilter filter, CancellationToken cancellationToken = default)
    {
        var query = _db.Payments.AsNoTracking().Include(x => x.Customer).Include(x => x.Estimate).Include(x => x.PaymentMethod).AsQueryable();
        query = ApplyPaymentFilters(query, filter);

        var rows = await query.OrderByDescending(x => x.PaymentDate).Select(x => new
        {
            x.PaymentNumber,
            x.PaymentDate,
            Customer = x.Customer.Name,
            Estimate = x.Estimate != null ? x.Estimate.EstimateNumber : null,
            x.Amount,
            Method = x.PaymentMethod.Name,
            x.TransactionReference,
            Status = x.Status
        }).ToListAsync(cancellationToken);

        return Workbook("Payments", ["Payment No", "Date", "Customer", "Estimate No", "Amount", "Payment Method", "Reference", "Status"], rows.Select(x => new object?[] { x.PaymentNumber, x.PaymentDate, x.Customer, x.Estimate, x.Amount, x.Method, x.TransactionReference, x.Status.ToString() }));
    }

    public async Task<byte[]> OutstandingAsync(ReportFilter filter, CancellationToken cancellationToken = default)
        => await BalanceReportAsync(filter, outstanding: true, cancellationToken);

    public async Task<byte[]> FullyPaidAsync(ReportFilter filter, CancellationToken cancellationToken = default)
        => await BalanceReportAsync(filter, outstanding: false, cancellationToken);

    public async Task<byte[]> MonthlyCollectionsAsync(ReportFilter filter, CancellationToken cancellationToken = default)
    {
        var query = _db.Payments.AsNoTracking().Where(x => x.Status == PaymentStatus.Posted).AsQueryable();
        query = ApplyPaymentFilters(query, filter);
        var rows = await query.GroupBy(x => new { x.PaymentDate.Year, x.PaymentDate.Month }).OrderBy(x => x.Key.Year).ThenBy(x => x.Key.Month)
            .Select(x => new { x.Key.Year, x.Key.Month, Total = x.Sum(p => p.Amount) }).ToListAsync(cancellationToken);
        return Workbook("Monthly Collections", ["Month", "Total Received"], rows.Select(x => new object?[] { $"{x.Year}-{x.Month:00}", x.Total }));
    }

    private async Task<byte[]> BalanceReportAsync(ReportFilter filter, bool outstanding, CancellationToken cancellationToken)
    {
        var query = _db.Customers.AsNoTracking().AsQueryable();
        query = ApplyCustomerSearch(query, filter);

        var rows = await query.Select(c => new
        {
            c.CustomerCode,
            Customer = c.Name,
            c.Phone,
            c.Location,
            EstimatedValue = c.Estimates.Where(e => ContributingStatuses.Contains(e.Status)).Sum(e => (decimal?)e.CalculatedTotalAmount) ?? 0m,
            Received = c.Payments.Where(p => p.Status == PaymentStatus.Posted).Sum(p => (decimal?)p.Amount) ?? 0m
        }).Where(x => outstanding ? x.EstimatedValue > x.Received : x.EstimatedValue > 0 && x.EstimatedValue <= x.Received)
        .OrderBy(x => x.Customer).ToListAsync(cancellationToken);

        return Workbook(outstanding ? "Outstanding Customers" : "Fully Paid Customers", ["Customer Code", "Customer", "Phone", "Location", "Estimated Value", "Received", "Outstanding"], rows.Select(x => new object?[] { x.CustomerCode, x.Customer, x.Phone, x.Location, x.EstimatedValue, x.Received, x.EstimatedValue - x.Received }));
    }

    private static IQueryable<Customer> ApplyCustomerSearch(IQueryable<Customer> query, ReportFilter filter)
    {
        if (filter.CustomerId.HasValue) query = query.Where(x => x.Id == filter.CustomerId.Value);
        if (!string.IsNullOrWhiteSpace(filter.Customer)) query = query.Where(x => x.Name.Contains(filter.Customer) || x.CustomerCode.Contains(filter.Customer) || x.Phone.Contains(filter.Customer));
        return query;
    }

    private static IQueryable<Payment> ApplyPaymentFilters(IQueryable<Payment> query, ReportFilter filter)
    {
        if (filter.CustomerId.HasValue) query = query.Where(x => x.CustomerId == filter.CustomerId.Value);
        if (!string.IsNullOrWhiteSpace(filter.Customer)) query = query.Where(x => x.Customer.Name.Contains(filter.Customer));
        if (filter.PaymentMethodId.HasValue) query = query.Where(x => x.PaymentMethodId == filter.PaymentMethodId.Value);
        if (filter.DateFrom.HasValue) query = query.Where(x => x.PaymentDate >= filter.DateFrom.Value);
        if (filter.DateTo.HasValue) query = query.Where(x => x.PaymentDate <= filter.DateTo.Value);
        return query;
    }

    private static byte[] Workbook(string title, IReadOnlyList<string> headers, IEnumerable<object?[]> rows)
    {
        using var workbook = new XLWorkbook();
        var sheet = workbook.Worksheets.Add("Report");
        sheet.Cell(1, 1).Value = title;
        sheet.Range(1, 1, 1, headers.Count).Merge();
        sheet.Cell(1, 1).Style.Font.Bold = true;
        sheet.Cell(1, 1).Style.Font.FontSize = 15;
        sheet.Cell(1, 1).Style.Font.FontColor = XLColor.FromHtml("#8b681f");

        for (var index = 0; index < headers.Count; index++)
        {
            var cell = sheet.Cell(3, index + 1);
            cell.Value = headers[index];
            cell.Style.Font.Bold = true;
            cell.Style.Font.FontColor = XLColor.White;
            cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#22252d");
        }

        var rowNumber = 4;
        foreach (var row in rows)
        {
            for (var column = 0; column < row.Length; column++)
            {
                var cell = sheet.Cell(rowNumber, column + 1);
                var value = row[column];
                if (value is DateTime date)
                {
                    cell.Value = date;
                    cell.Style.DateFormat.Format = "dd mmm yyyy";
                }
                else if (value is decimal amount)
                {
                    cell.Value = amount;
                    cell.Style.NumberFormat.Format = "₹ #,##0.00";
                }
                else
                {
                    cell.Value = value?.ToString() ?? string.Empty;
                }
            }
            rowNumber++;
        }

        sheet.SheetView.FreezeRows(3);
        sheet.Columns().AdjustToContents();
        foreach (var column in sheet.ColumnsUsed())
        {
            if (column.Width > 36) column.Width = 36;
        }

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}
