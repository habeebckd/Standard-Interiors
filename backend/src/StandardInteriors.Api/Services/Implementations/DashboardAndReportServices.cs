using Microsoft.EntityFrameworkCore;
using StandardInteriors.Api.Data;
using StandardInteriors.Api.Domain.Enums;
using StandardInteriors.Api.Dtos.Dashboard;
using StandardInteriors.Api.Dtos.Payments;
using StandardInteriors.Api.Dtos.Reports;

namespace StandardInteriors.Api.Services.Implementations;

public interface IDashboardService
{
    Task<DashboardDto> GetAsync(CancellationToken cancellationToken = default);
}

public interface IReportService
{
    Task<IReadOnlyList<AllCustomerReportRowDto>> GetCustomersAsync(ReportFilter? filter = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AllEstimateReportRowDto>> GetEstimatesAsync(ReportFilter? filter = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AllPaymentReportRowDto>> GetPaymentsAsync(ReportFilter? filter = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<OutstandingReportRowDto>> GetOutstandingAsync(ReportFilter? filter = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MonthlyCollectionReportRowDto>> GetMonthlyCollectionsAsync(ReportFilter? filter = null, CancellationToken cancellationToken = default);
}

public sealed class DashboardService : IDashboardService
{
    private static readonly EstimateStatus[] ContributingStatuses = [EstimateStatus.Sent, EstimateStatus.Approved, EstimateStatus.Completed];
    private readonly StandardInteriorsDbContext _db;

    public DashboardService(StandardInteriorsDbContext db) => _db = db;

    public async Task<DashboardDto> GetAsync(CancellationToken cancellationToken = default)
    {
        var totalCustomers = await _db.Customers.CountAsync(cancellationToken);
        var totalEstimates = await _db.Estimates.CountAsync(cancellationToken);
        var totalEstimatedValue = await _db.Estimates.Where(x => ContributingStatuses.Contains(x.Status)).SumAsync(x => (decimal?)x.CalculatedTotalAmount, cancellationToken) ?? 0m;
        var totalAmountReceived = await _db.Payments.Where(x => x.Status == Domain.Enums.PaymentStatus.Posted).SumAsync(x => (decimal?)x.Amount, cancellationToken) ?? 0m;
        var totalOutstanding = await _db.Estimates.Where(x => ContributingStatuses.Contains(x.Status)).SumAsync(x => (decimal?)x.CalculatedBalance, cancellationToken) ?? 0m;
        var thisMonthReceived = await _db.Payments.Where(x => x.Status == Domain.Enums.PaymentStatus.Posted && x.PaymentDate.Month == DateTime.Now.Month && x.PaymentDate.Year == DateTime.Now.Year).SumAsync(x => (decimal?)x.Amount, cancellationToken) ?? 0m;

        var monthlyTrend = await _db.Payments.Where(x => x.Status == Domain.Enums.PaymentStatus.Posted)
            .GroupBy(x => new { x.PaymentDate.Year, x.PaymentDate.Month })
            .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
            .Select(g => new MonthlyTrendPointDto($"{g.Key.Year}-{g.Key.Month:00}", g.Sum(x => x.Amount)))
            .Take(12)
            .ToListAsync(cancellationToken);

        var recentEstimatesRaw = await _db.Estimates.AsNoTracking().Include(x => x.Customer).OrderByDescending(x => x.EstimateDate).Take(5)
            .Select(x => new { x.EstimateNumber, CustomerName = x.Customer.Name, x.EstimateDate, x.CalculatedTotalAmount, x.Status, x.CalculatedBalance })
            .ToListAsync(cancellationToken);
        var recentEstimates = recentEstimatesRaw
            .Select(x => new RecentEstimateDto(x.EstimateNumber, x.CustomerName, x.EstimateDate, x.CalculatedTotalAmount, x.Status.ToString(), x.CalculatedBalance))
            .ToList();

        var recentPaymentsRaw = await _db.Payments.AsNoTracking().Include(x => x.Customer).Include(x => x.PaymentMethod).Where(x => x.Status == Domain.Enums.PaymentStatus.Posted).OrderByDescending(x => x.PaymentDate).Take(5)
            .Select(x => new { x.PaymentDate, CustomerName = x.Customer.Name, x.Amount, MethodName = x.PaymentMethod.Name, x.TransactionReference })
            .ToListAsync(cancellationToken);
        var recentPayments = recentPaymentsRaw
            .Select(x => new RecentPaymentDto(x.PaymentDate, x.CustomerName, x.Amount, x.MethodName, x.TransactionReference))
            .ToList();

        var outstandingCustomers = (await _db.Customers.AsNoTracking()
            .Select(c => new OutstandingCustomerDto(
                c.Name,
                c.Estimates.Where(e => ContributingStatuses.Contains(e.Status)).Sum(e => (decimal?)e.CalculatedTotalAmount) ?? 0m,
                c.Payments.Where(p => p.Status == Domain.Enums.PaymentStatus.Posted).Sum(p => (decimal?)p.Amount) ?? 0m,
                (c.Estimates.Where(e => ContributingStatuses.Contains(e.Status)).Sum(e => (decimal?)e.CalculatedTotalAmount) ?? 0m) - (c.Payments.Where(p => p.Status == Domain.Enums.PaymentStatus.Posted).Sum(p => (decimal?)p.Amount) ?? 0m)))
            .ToListAsync(cancellationToken))
            .Where(x => x.Balance > 0)
            .OrderByDescending(x => x.Balance)
            .Take(5)
            .ToList();

        var workStatuses = await _db.Estimates.AsNoTracking().Select(x => x.WorkStatus).ToListAsync(cancellationToken);
        var workStatusSummary = workStatuses.GroupBy(x => x).OrderBy(x => x.Key).Select(x => new WorkStatusCountDto(x.Key, x.Count())).ToList();

        return new DashboardDto(totalCustomers, totalEstimates, totalEstimatedValue, totalAmountReceived, totalOutstanding, thisMonthReceived, monthlyTrend, recentEstimates, recentPayments, outstandingCustomers, workStatusSummary);
    }
}

public sealed class ReportService : IReportService
{
    private static readonly EstimateStatus[] ContributingStatuses = [EstimateStatus.Sent, EstimateStatus.Approved, EstimateStatus.Completed];
    private readonly StandardInteriorsDbContext _db;

    public ReportService(StandardInteriorsDbContext db) => _db = db;

    public async Task<IReadOnlyList<AllCustomerReportRowDto>> GetCustomersAsync(ReportFilter? filter = null, CancellationToken cancellationToken = default)
    {
        var query = _db.Customers.AsNoTracking().AsQueryable();
        if (filter?.CustomerId is int customerId) query = query.Where(x => x.Id == customerId);
        if (!string.IsNullOrWhiteSpace(filter?.Customer)) query = query.Where(x => x.Name.Contains(filter.Customer) || x.CustomerCode.Contains(filter.Customer) || x.Phone.Contains(filter.Customer));

        var rows = await query.OrderBy(x => x.Name)
            .Select(c => new AllCustomerReportRowDto(c.CustomerCode, c.Name, c.Phone, c.Location, c.Estimates.Count(e => ContributingStatuses.Contains(e.Status)), c.Estimates.Where(e => ContributingStatuses.Contains(e.Status)).Sum(e => (decimal?)e.CalculatedTotalAmount) ?? 0m, c.Payments.Where(p => p.Status == Domain.Enums.PaymentStatus.Posted).Sum(p => (decimal?)p.Amount) ?? 0m, (c.Estimates.Where(e => ContributingStatuses.Contains(e.Status)).Sum(e => (decimal?)e.CalculatedTotalAmount) ?? 0m) - (c.Payments.Where(p => p.Status == Domain.Enums.PaymentStatus.Posted).Sum(p => (decimal?)p.Amount) ?? 0m)))
            .ToListAsync(cancellationToken);
        if (filter?.HasOutstanding == true) rows = rows.Where(x => x.Outstanding > 0).ToList();
        if (filter?.FullyPaid == true) rows = rows.Where(x => x.TotalEstimates > 0 && x.Outstanding <= 0).ToList();
        return rows;
    }

    public async Task<IReadOnlyList<AllEstimateReportRowDto>> GetEstimatesAsync(ReportFilter? filter = null, CancellationToken cancellationToken = default)
        {
            var query = _db.Estimates.AsNoTracking().Include(x => x.Customer).AsQueryable();
            if (filter?.CustomerId is int customerId) query = query.Where(x => x.CustomerId == customerId);
            if (!string.IsNullOrWhiteSpace(filter?.Customer)) query = query.Where(x => x.Customer.Name.Contains(filter.Customer));
            if (filter?.EstimateStatus is EstimateStatus status) query = query.Where(x => x.Status == status);
            if (filter?.DateFrom is DateTime dateFrom) query = query.Where(x => x.EstimateDate >= dateFrom);
            if (filter?.DateTo is DateTime dateTo) query = query.Where(x => x.EstimateDate <= dateTo);

            var raw = await query.OrderByDescending(x => x.EstimateDate)
                .Select(x => new { x.EstimateNumber, CustomerName = x.Customer.Name, x.EstimateDate, x.Status, x.CalculatedTotalAmount, x.CalculatedReceivedAmount, x.CalculatedBalance })
                .ToListAsync(cancellationToken);
            return raw.Select(x => new AllEstimateReportRowDto(x.EstimateNumber, x.CustomerName, x.EstimateDate, x.Status.ToString(), x.CalculatedTotalAmount, x.CalculatedReceivedAmount, x.CalculatedBalance)).ToList();
        }

    public async Task<IReadOnlyList<AllPaymentReportRowDto>> GetPaymentsAsync(ReportFilter? filter = null, CancellationToken cancellationToken = default)
        {
            var query = _db.Payments.AsNoTracking().Include(x => x.Customer).Include(x => x.Estimate).Include(x => x.PaymentMethod).AsQueryable();
            if (filter?.CustomerId is int customerId) query = query.Where(x => x.CustomerId == customerId);
            if (!string.IsNullOrWhiteSpace(filter?.Customer)) query = query.Where(x => x.Customer.Name.Contains(filter.Customer));
            if (filter?.PaymentMethodId is int methodId) query = query.Where(x => x.PaymentMethodId == methodId);
            if (filter?.DateFrom is DateTime dateFrom) query = query.Where(x => x.PaymentDate >= dateFrom);
            if (filter?.DateTo is DateTime dateTo) query = query.Where(x => x.PaymentDate <= dateTo);

            var raw = await query.OrderByDescending(x => x.PaymentDate)
                .Select(x => new { x.PaymentNumber, x.PaymentDate, CustomerName = x.Customer.Name, EstimateNumber = x.Estimate != null ? x.Estimate.EstimateNumber : null, x.Amount, PaymentMethod = x.PaymentMethod.Name, x.TransactionReference, x.Status })
                .ToListAsync(cancellationToken);
            return raw.Select(x => new AllPaymentReportRowDto(x.PaymentNumber, x.PaymentDate, x.CustomerName, x.EstimateNumber, x.Amount, x.PaymentMethod, x.TransactionReference, x.Status.ToString())).ToList();
        }

    public async Task<IReadOnlyList<OutstandingReportRowDto>> GetOutstandingAsync(ReportFilter? filter = null, CancellationToken cancellationToken = default)
        {
            var customerQuery = _db.Customers.AsNoTracking().AsQueryable();
            if (filter?.CustomerId is int customerId) customerQuery = customerQuery.Where(x => x.Id == customerId);
            if (!string.IsNullOrWhiteSpace(filter?.Customer)) customerQuery = customerQuery.Where(x => x.Name.Contains(filter.Customer) || x.CustomerCode.Contains(filter.Customer) || x.Phone.Contains(filter.Customer));
            var query = customerQuery.OrderBy(x => x.Name).Select(c => new
            {
                c.CustomerCode,
                CustomerName = c.Name,
                c.Phone,
                c.Location,
                EstimatedValue = c.Estimates.Where(e => ContributingStatuses.Contains(e.Status)).Sum(e => (decimal?)e.CalculatedTotalAmount) ?? 0m,
                Received = c.Payments.Where(p => p.Status == Domain.Enums.PaymentStatus.Posted).Sum(p => (decimal?)p.Amount) ?? 0m
            }).Where(x => x.EstimatedValue > x.Received);

            var raw = await query.ToListAsync(cancellationToken);
            return raw.Select(x => new OutstandingReportRowDto(x.CustomerCode, x.CustomerName, x.Phone, x.Location, x.EstimatedValue, x.Received, x.EstimatedValue - x.Received)).ToList();
        }

    public async Task<IReadOnlyList<MonthlyCollectionReportRowDto>> GetMonthlyCollectionsAsync(ReportFilter? filter = null, CancellationToken cancellationToken = default)
    {
        var query = _db.Payments.AsNoTracking().Where(x => x.Status == Domain.Enums.PaymentStatus.Posted).AsQueryable();
        if (filter?.CustomerId is int customerId) query = query.Where(x => x.CustomerId == customerId);
        if (!string.IsNullOrWhiteSpace(filter?.Customer)) query = query.Where(x => x.Customer.Name.Contains(filter.Customer));
        if (filter?.PaymentMethodId is int methodId) query = query.Where(x => x.PaymentMethodId == methodId);
        if (filter?.DateFrom is DateTime dateFrom) query = query.Where(x => x.PaymentDate >= dateFrom);
        if (filter?.DateTo is DateTime dateTo) query = query.Where(x => x.PaymentDate <= dateTo);
        return await query
            .GroupBy(x => new { x.PaymentDate.Year, x.PaymentDate.Month })
            .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
            .Select(g => new MonthlyCollectionReportRowDto($"{g.Key.Year}-{g.Key.Month:00}", g.Sum(x => x.Amount), 0m))
            .ToListAsync(cancellationToken);
    }
}
