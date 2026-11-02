using Microsoft.EntityFrameworkCore;
using StandardInteriors.Api.Data;
using StandardInteriors.Api.Domain.Entities;
using StandardInteriors.Api.Domain.Enums;
using StandardInteriors.Api.Dtos.Common;
using StandardInteriors.Api.Dtos.Customers;
using StandardInteriors.Api.Dtos.Estimates;
using StandardInteriors.Api.Dtos.Payments;

namespace StandardInteriors.Api.Services.Implementations;

public interface ICustomerService
{
    Task<PagedResult<CustomerListItemDto>> GetCustomersAsync(PagingQuery query, bool? hasOutstanding, bool? fullyPaid, bool? hasEstimates, DateTime? from, DateTime? to, CancellationToken cancellationToken = default);
    Task<CustomerDetailDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<CustomerSummaryDto?> GetSummaryAsync(int id, CancellationToken cancellationToken = default);
    Task<CustomerStatementDto?> GetStatementAsync(int id, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(CustomerUpsertDto dto, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(int id, CustomerUpsertDto dto, CancellationToken cancellationToken = default);
}

public sealed class CustomerService : ICustomerService
{
    private static readonly EstimateStatus[] ContributingEstimateStatuses = [EstimateStatus.Sent, EstimateStatus.Approved, EstimateStatus.Completed];
    private readonly StandardInteriorsDbContext _db;
    private readonly ICalculationService _calculator;
    private readonly ICurrentUserService _currentUser;

    public CustomerService(StandardInteriorsDbContext db, ICalculationService calculator, ICurrentUserService currentUser)
    {
        _db = db;
        _calculator = calculator;
        _currentUser = currentUser;
    }

    public async Task<PagedResult<CustomerListItemDto>> GetCustomersAsync(PagingQuery query, bool? hasOutstanding, bool? fullyPaid, bool? hasEstimates, DateTime? from, DateTime? to, CancellationToken cancellationToken = default)
    {
        var listQuery = _db.Customers.AsNoTracking().Select(c => new
        {
            c.Id,
            c.CustomerCode,
            c.Name,
            c.Phone,
            c.AlternatePhone,
            c.Location,
            TotalEstimateValue = c.Estimates.Where(e => ContributingEstimateStatuses.Contains(e.Status)).Sum(e => (decimal?)e.CalculatedTotalAmount) ?? 0m,
            TotalReceived = c.Payments.Where(p => p.Status == PaymentStatus.Posted).Sum(p => (decimal?)p.Amount) ?? 0m,
            EstimateCount = c.Estimates.Count(e => ContributingEstimateStatuses.Contains(e.Status)),
            LastActivity = c.UpdatedAt,
            LatestWorkStatus = c.Estimates.OrderByDescending(e => e.UpdatedAt).Select(e => (WorkStatus?)e.WorkStatus).FirstOrDefault(),
            LatestWorkProgress = c.Estimates.OrderByDescending(e => e.UpdatedAt).Select(e => (int?)e.ProgressPercentage).FirstOrDefault()
        });

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim();
            listQuery = listQuery.Where(x => x.CustomerCode.Contains(search) || x.Name.Contains(search) || x.Phone.Contains(search) || (x.AlternatePhone != null && x.AlternatePhone.Contains(search)) || (x.Location != null && x.Location.Contains(search)));
        }

        if (hasOutstanding == true)
        {
            listQuery = listQuery.Where(x => x.TotalEstimateValue - x.TotalReceived > 0);
        }
        else if (fullyPaid == true)
        {
            listQuery = listQuery.Where(x => x.TotalEstimateValue - x.TotalReceived <= 0 && x.EstimateCount > 0);
        }

        if (hasEstimates == true)
        {
            listQuery = listQuery.Where(x => x.EstimateCount > 0);
        }

        if (from.HasValue)
        {
            listQuery = listQuery.Where(x => x.LastActivity >= from.Value);
        }

        if (to.HasValue)
        {
            listQuery = listQuery.Where(x => x.LastActivity <= to.Value);
        }

        listQuery = query.SortBy?.ToLowerInvariant() switch
        {
            "name" => query.SortDesc ? listQuery.OrderByDescending(x => x.Name) : listQuery.OrderBy(x => x.Name),
            "outstanding" => query.SortDesc ? listQuery.OrderByDescending(x => x.TotalEstimateValue - x.TotalReceived) : listQuery.OrderBy(x => x.TotalEstimateValue - x.TotalReceived),
            "activity" => query.SortDesc ? listQuery.OrderByDescending(x => x.LastActivity) : listQuery.OrderBy(x => x.LastActivity),
            _ => listQuery.OrderBy(x => x.Name)
        };

        var totalCount = await listQuery.CountAsync(cancellationToken);
        var page = Math.Max(query.Page, 1);
        var pageSize = Math.Clamp(query.PageSize, 1, 200);

        var rows = await listQuery.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
        var items = rows.Select(x => new CustomerListItemDto(x.Id, x.CustomerCode, x.Name, x.Phone, x.AlternatePhone, x.Location, x.TotalEstimateValue, x.TotalReceived, Math.Max(x.TotalEstimateValue - x.TotalReceived, 0m), x.EstimateCount, x.LastActivity, x.LatestWorkStatus, x.LatestWorkProgress)).ToList();
        return new PagedResult<CustomerListItemDto>(items, totalCount, page, pageSize);
    }

    public async Task<CustomerDetailDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var summary = await GetSummaryAsync(id, cancellationToken);
        var customer = await _db.Customers.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (customer == null || summary == null)
        {
            return null;
        }

        return new CustomerDetailDto(customer.Id, customer.CustomerCode, customer.Name, customer.Phone, customer.AlternatePhone, customer.Email, customer.Address, customer.Location, customer.Notes, customer.IsActive, summary);
    }

    public async Task<CustomerSummaryDto?> GetSummaryAsync(int id, CancellationToken cancellationToken = default)
    {
        var customer = await _db.Customers.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (customer == null)
        {
            return null;
        }

        var estimates = await _db.Estimates.AsNoTracking()
            .Where(x => x.CustomerId == id && ContributingEstimateStatuses.Contains(x.Status))
            .Select(x => new { x.Id, x.CalculatedTotalAmount, x.CalculatedReceivedAmount, x.CalculatedBalance })
            .ToListAsync(cancellationToken);

        var payments = await _db.Payments.AsNoTracking().Where(x => x.CustomerId == id && x.Status == PaymentStatus.Posted).ToListAsync(cancellationToken);

        var totalEstimated = estimates.Sum(x => x.CalculatedTotalAmount);
        var totalReceived = payments.Sum(x => x.Amount);
        var outstanding = _calculator.CalculateOutstanding(totalEstimated, totalReceived);

        return new CustomerSummaryDto(customer.Id, customer.CustomerCode, customer.Name, customer.Phone, customer.AlternatePhone, customer.Email, customer.Address, customer.Location, customer.Notes, totalEstimated, totalReceived, outstanding, estimates.Count, payments.Count);
    }

    public async Task<CustomerStatementDto?> GetStatementAsync(int id, CancellationToken cancellationToken = default)
    {
        var summary = await GetSummaryAsync(id, cancellationToken);
        if (summary == null)
        {
            return null;
        }

        var estimateLines = await _db.Estimates.AsNoTracking()
            .Where(x => x.CustomerId == id && ContributingEstimateStatuses.Contains(x.Status))
            .OrderBy(x => x.EstimateDate)
            .Select(x => new CustomerStatementLineDto(x.EstimateDate, x.EstimateNumber, "Estimate", x.CalculatedTotalAmount, 0m, 0m))
            .ToListAsync(cancellationToken);

        var paymentLines = await _db.Payments.AsNoTracking()
            .Where(x => x.CustomerId == id && x.Status == PaymentStatus.Posted)
            .OrderBy(x => x.PaymentDate)
            .Select(x => new CustomerStatementLineDto(x.PaymentDate, x.PaymentNumber, "Payment", 0m, x.Amount, 0m))
            .ToListAsync(cancellationToken);

        var lines = estimateLines.Concat(paymentLines).OrderBy(x => x.Date).ThenBy(x => x.Reference).ToList();
        decimal running = 0m;
        for (var index = 0; index < lines.Count; index++)
        {
            var line = lines[index];
            running += line.Debit;
            running -= line.Credit;
            lines[index] = line with { RunningBalance = running };
        }

        return new CustomerStatementDto(summary, lines);
    }

    public async Task<int> CreateAsync(CustomerUpsertDto dto, CancellationToken cancellationToken = default)
    {
        var nextCode = await GenerateCustomerCodeAsync(cancellationToken);
        var customer = new Customer
        {
            CustomerCode = nextCode,
            Name = dto.Name.Trim(),
            Phone = dto.Phone.Trim(),
            AlternatePhone = dto.AlternatePhone?.Trim(),
            Email = dto.Email?.Trim(),
            Address = dto.Address?.Trim(),
            Location = dto.Location?.Trim(),
            Notes = dto.Notes?.Trim(),
            IsActive = dto.IsActive
        };
        customer.CreatedByUserId = _currentUser.UserId;
        customer.UpdatedByUserId = _currentUser.UserId;

        _db.Customers.Add(customer);
        await _db.SaveChangesAsync(cancellationToken);
        return customer.Id;
    }

    public async Task<bool> UpdateAsync(int id, CustomerUpsertDto dto, CancellationToken cancellationToken = default)
    {
        var customer = await _db.Customers.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (customer == null)
        {
            return false;
        }

        customer.Name = dto.Name.Trim();
        customer.Phone = dto.Phone.Trim();
        customer.AlternatePhone = dto.AlternatePhone?.Trim();
        customer.Email = dto.Email?.Trim();
        customer.Address = dto.Address?.Trim();
        customer.Location = dto.Location?.Trim();
        customer.Notes = dto.Notes?.Trim();
        customer.IsActive = dto.IsActive;
        customer.UpdatedByUserId = _currentUser.UserId;
        customer.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }

    private async Task<string> GenerateCustomerCodeAsync(CancellationToken cancellationToken)
    {
        var count = await _db.Customers.CountAsync(cancellationToken) + 1;
        return $"CUS-{count:000000}";
    }
}
