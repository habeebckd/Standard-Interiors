using Microsoft.EntityFrameworkCore;
using StandardInteriors.Api.Data;
using StandardInteriors.Api.Domain.Entities;
using StandardInteriors.Api.Domain.Enums;
using StandardInteriors.Api.Dtos.Common;
using StandardInteriors.Api.Dtos.Payments;

namespace StandardInteriors.Api.Services.Implementations;

public interface IPaymentService
{
    Task<PagedResult<PaymentListItemDto>> GetPaymentsAsync(PagingQuery query, int? customerId = null, int? estimateId = null, PaymentStatus? status = null, DateTime? from = null, DateTime? to = null, CancellationToken cancellationToken = default);
    Task<PaymentDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(PaymentUpsertDto dto, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(int id, PaymentUpsertDto dto, CancellationToken cancellationToken = default);
    Task<bool> VoidAsync(int id, string reason, CancellationToken cancellationToken = default);
    Task<PaymentReceiptDto?> GetReceiptAsync(int id, CancellationToken cancellationToken = default);
    Task<PaymentSheetDto?> GetSheetAsync(int customerId, CancellationToken cancellationToken = default);
}

public sealed class PaymentService : IPaymentService
{
    private readonly StandardInteriorsDbContext _db;
    private readonly ICalculationService _calculator;
    private readonly IEstimateService _estimateService;
    private readonly ICustomerService _customerService;
    private readonly ICurrentUserService _currentUser;

    public PaymentService(StandardInteriorsDbContext db, ICalculationService calculator, IEstimateService estimateService, ICustomerService customerService, ICurrentUserService currentUser)
    {
        _db = db;
        _calculator = calculator;
        _estimateService = estimateService;
        _customerService = customerService;
        _currentUser = currentUser;
    }

    public async Task<PagedResult<PaymentListItemDto>> GetPaymentsAsync(PagingQuery query, int? customerId = null, int? estimateId = null, PaymentStatus? status = null, DateTime? from = null, DateTime? to = null, CancellationToken cancellationToken = default)
    {
        var listQuery = _db.Payments.AsNoTracking().Include(x => x.Customer).Include(x => x.Estimate).Include(x => x.PaymentMethod).AsQueryable();

        if (customerId.HasValue) listQuery = listQuery.Where(x => x.CustomerId == customerId.Value);
        if (estimateId.HasValue) listQuery = listQuery.Where(x => x.EstimateId == estimateId.Value);
        if (status.HasValue) listQuery = listQuery.Where(x => x.Status == status.Value);
        if (from.HasValue) listQuery = listQuery.Where(x => x.PaymentDate >= from.Value);
        if (to.HasValue) listQuery = listQuery.Where(x => x.PaymentDate <= to.Value);
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim();
            listQuery = listQuery.Where(x => x.PaymentNumber.Contains(search) || x.Customer.Name.Contains(search) || (x.TransactionReference != null && x.TransactionReference.Contains(search)));
        }

        var projected = listQuery.Select(x => new
        {
            x.Id,
            x.PaymentNumber,
            x.PaymentDate,
            CustomerName = x.Customer.Name,
            EstimateNumber = x.Estimate != null ? x.Estimate.EstimateNumber : null,
            x.Amount,
            PaymentMethodName = x.PaymentMethod.Name,
            x.TransactionReference,
            x.Notes,
            x.Status
        });
        projected = query.SortBy?.ToLowerInvariant() switch
        {
            "amount" => query.SortDesc ? projected.OrderByDescending(x => x.Amount) : projected.OrderBy(x => x.Amount),
            "date" => query.SortDesc ? projected.OrderByDescending(x => x.PaymentDate) : projected.OrderBy(x => x.PaymentDate),
            _ => projected.OrderByDescending(x => x.PaymentDate)
        };

        var totalCount = await projected.CountAsync(cancellationToken);
        var page = Math.Max(query.Page, 1);
        var pageSize = Math.Clamp(query.PageSize, 1, 200);
        var rows = await projected.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
        var items = rows.Select(x => new PaymentListItemDto(x.Id, x.PaymentNumber, x.PaymentDate, x.CustomerName, x.EstimateNumber, x.Amount, x.PaymentMethodName, x.TransactionReference, x.Notes, x.Status)).ToList();
        return new PagedResult<PaymentListItemDto>(items, totalCount, page, pageSize);
    }

    public async Task<PaymentDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _db.Payments.AsNoTracking().Include(x => x.Customer).Include(x => x.Estimate).Include(x => x.PaymentMethod)
            .Where(x => x.Id == id)
            .Select(x => new PaymentDto(x.Id, x.PaymentNumber, x.CustomerId, x.Customer.Name, x.EstimateId, x.Estimate != null ? x.Estimate.EstimateNumber : null, x.PaymentDate, x.Amount, x.PaymentMethodId, x.PaymentMethod.Name, x.TransactionReference, x.Notes, x.Status, x.VoidedAt, x.VoidReason))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<int> CreateAsync(PaymentUpsertDto dto, CancellationToken cancellationToken = default)
    {
        ValidatePayment(dto);
        var estimate = dto.EstimateId.HasValue ? await _db.Estimates.FirstOrDefaultAsync(x => x.Id == dto.EstimateId.Value, cancellationToken) : null;
        if (estimate != null && estimate.CustomerId != dto.CustomerId) throw new InvalidOperationException("Selected estimate does not belong to the customer.");

        var outstanding = await GetTargetOutstandingAsync(dto.CustomerId, dto.EstimateId, cancellationToken);
        if (!dto.AllowOverpayment && dto.Amount > outstanding)
        {
            throw new InvalidOperationException($"Payment exceeds outstanding balance ({outstanding:0.00}).");
        }

        var payment = new Payment
        {
            PaymentNumber = await GeneratePaymentNumberAsync(cancellationToken),
            CustomerId = dto.CustomerId,
            EstimateId = dto.EstimateId,
            PaymentDate = dto.PaymentDate,
            Amount = dto.Amount,
            PaymentMethodId = dto.PaymentMethodId,
            TransactionReference = dto.TransactionReference?.Trim(),
            Notes = dto.Notes?.Trim(),
            Status = PaymentStatus.Posted,
            CreatedByUserId = _currentUser.UserId,
            UpdatedByUserId = _currentUser.UserId
        };

        _db.Payments.Add(payment);
        await _db.SaveChangesAsync(cancellationToken);

        if (payment.EstimateId.HasValue)
        {
            await _estimateService.RecalculateAsync(payment.EstimateId.Value, cancellationToken);
        }

        return payment.Id;
    }

    public async Task<bool> UpdateAsync(int id, PaymentUpsertDto dto, CancellationToken cancellationToken = default)
    {
        var payment = await _db.Payments.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (payment == null || payment.Status == PaymentStatus.Voided) return false;

        ValidatePayment(dto);
        if (payment.CustomerId != dto.CustomerId) throw new InvalidOperationException("Customer cannot be changed for posted payment.");

        var estimate = dto.EstimateId.HasValue ? await _db.Estimates.FirstOrDefaultAsync(x => x.Id == dto.EstimateId.Value, cancellationToken) : null;
        if (estimate != null && estimate.CustomerId != dto.CustomerId) throw new InvalidOperationException("Selected estimate does not belong to the customer.");

        payment.EstimateId = dto.EstimateId;
        payment.PaymentDate = dto.PaymentDate;
        payment.Amount = dto.Amount;
        payment.PaymentMethodId = dto.PaymentMethodId;
        payment.TransactionReference = dto.TransactionReference?.Trim();
        payment.Notes = dto.Notes?.Trim();
        payment.UpdatedAt = DateTime.UtcNow;
        payment.UpdatedByUserId = _currentUser.UserId;

        await _db.SaveChangesAsync(cancellationToken);
        if (payment.EstimateId.HasValue)
        {
            await _estimateService.RecalculateAsync(payment.EstimateId.Value, cancellationToken);
        }

        return true;
    }

    public async Task<bool> VoidAsync(int id, string reason, CancellationToken cancellationToken = default)
    {
        var payment = await _db.Payments.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (payment == null || payment.Status == PaymentStatus.Voided) return false;

        payment.Status = PaymentStatus.Voided;
        payment.VoidedAt = DateTime.UtcNow;
        payment.VoidReason = reason.Trim();
        payment.VoidedByUserId = _currentUser.UserId;
        payment.UpdatedByUserId = _currentUser.UserId;
        payment.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);

        if (payment.EstimateId.HasValue)
        {
            await _estimateService.RecalculateAsync(payment.EstimateId.Value, cancellationToken);
        }

        return true;
    }

    public async Task<PaymentReceiptDto?> GetReceiptAsync(int id, CancellationToken cancellationToken = default)
    {
        var payment = await _db.Payments.AsNoTracking().Include(x => x.Customer).Include(x => x.Estimate).Include(x => x.PaymentMethod).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (payment == null) return null;

        var targetOutstanding = await GetTargetOutstandingAsync(payment.CustomerId, payment.EstimateId, cancellationToken, excludePaymentId: payment.Id);
        var previousBalance = targetOutstanding + (payment.Status == PaymentStatus.Posted ? payment.Amount : 0m);
        var remaining = payment.Status == PaymentStatus.Posted ? Math.Max(targetOutstanding, 0m) : targetOutstanding;

        return new PaymentReceiptDto(payment.PaymentNumber, payment.PaymentDate, payment.Customer.Name, payment.Estimate?.EstimateNumber, previousBalance, payment.Amount, remaining, payment.PaymentMethod.Name, payment.TransactionReference, payment.Notes);
    }

    public async Task<PaymentSheetDto?> GetSheetAsync(int customerId, CancellationToken cancellationToken = default)
    {
        var customer = await _customerService.GetSummaryAsync(customerId, cancellationToken);
        if (customer == null) return null;

        var payments = await _db.Payments.AsNoTracking().Include(x => x.PaymentMethod).Include(x => x.Estimate)
            .Where(x => x.CustomerId == customerId && x.Status == PaymentStatus.Posted)
            .OrderBy(x => x.PaymentDate)
            .Select(x => new PaymentListItemDto(x.Id, x.PaymentNumber, x.PaymentDate, x.Customer.Name, x.Estimate != null ? x.Estimate.EstimateNumber : null, x.Amount, x.PaymentMethod.Name, x.TransactionReference, x.Notes, x.Status))
            .ToListAsync(cancellationToken);

        var totalPaid = _calculator.CalculatePaymentTotal(await _db.Payments.AsNoTracking().Where(x => x.CustomerId == customerId).ToListAsync(cancellationToken));
        return new PaymentSheetDto(customer.Name, payments.FirstOrDefault()?.EstimateNumber, customer.TotalEstimated, totalPaid, customer.Outstanding, payments);
    }

    private async Task<decimal> GetTargetOutstandingAsync(int customerId, int? estimateId, CancellationToken cancellationToken, int? excludePaymentId = null)
    {
        if (estimateId.HasValue)
        {
            var estimate = await _db.Estimates.AsNoTracking().FirstOrDefaultAsync(x => x.Id == estimateId.Value, cancellationToken);
            if (estimate == null) return 0m;

            var payments = _db.Payments.AsNoTracking().Where(x => x.EstimateId == estimateId.Value && x.Status == PaymentStatus.Posted);
            if (excludePaymentId.HasValue) payments = payments.Where(x => x.Id != excludePaymentId.Value);
            var received = await payments.SumAsync(x => (decimal?)x.Amount, cancellationToken) ?? 0m;
            return Math.Max(estimate.CalculatedTotalAmount - received, 0m);
        }

        var summary = await _customerService.GetSummaryAsync(customerId, cancellationToken);
        if (summary == null) return 0m;

        if (!excludePaymentId.HasValue)
        {
            return summary.Outstanding;
        }

        var paymentToRemove = await _db.Payments.AsNoTracking().FirstOrDefaultAsync(x => x.Id == excludePaymentId.Value, cancellationToken);
        return paymentToRemove == null ? summary.Outstanding : summary.Outstanding + (paymentToRemove.Status == PaymentStatus.Posted ? paymentToRemove.Amount : 0m);
    }

    private async Task<string> GeneratePaymentNumberAsync(CancellationToken cancellationToken)
    {
        var year = DateTime.Now.Year;
        var next = await _db.Payments.CountAsync(cancellationToken) + 1;
        return $"PAY-{year}-{next:00000}";
    }

    private static void ValidatePayment(PaymentUpsertDto dto)
    {
        if (dto.CustomerId <= 0) throw new ArgumentException("Customer is required.");
        if (dto.Amount <= 0) throw new ArgumentException("Amount must be greater than zero.");
        if (dto.PaymentDate == default) throw new ArgumentException("Payment date is required.");
        if (dto.PaymentMethodId <= 0) throw new ArgumentException("Payment method is required.");
    }
}
