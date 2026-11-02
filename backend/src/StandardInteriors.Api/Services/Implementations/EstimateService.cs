using Microsoft.EntityFrameworkCore;
using StandardInteriors.Api.Data;
using StandardInteriors.Api.Domain.Entities;
using StandardInteriors.Api.Domain.Enums;
using StandardInteriors.Api.Dtos.Common;
using StandardInteriors.Api.Dtos.Customers;
using StandardInteriors.Api.Dtos.Estimates;

namespace StandardInteriors.Api.Services.Implementations;

public interface IEstimateService
{
    Task<PagedResult<EstimateListItemDto>> GetEstimatesAsync(PagingQuery query, int? customerId = null, EstimateStatus? status = null, DateTime? from = null, DateTime? to = null, CancellationToken cancellationToken = default);
    Task<EstimateDetailDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(EstimateUpsertDto dto, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(int id, EstimateUpsertDto dto, CancellationToken cancellationToken = default);
    Task<int?> DuplicateAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ChangeStatusAsync(int id, EstimateStatusChangeDto dto, CancellationToken cancellationToken = default);
    Task<bool> RecalculateAsync(int estimateId, CancellationToken cancellationToken = default);
}

public sealed class EstimateService : IEstimateService
{
    private static readonly EstimateStatus[] ContributingStatuses = [EstimateStatus.Sent, EstimateStatus.Approved, EstimateStatus.Completed];
    private readonly StandardInteriorsDbContext _db;
    private readonly ICalculationService _calculator;
    private readonly ISettingsService _settings;
    private readonly ICurrentUserService _currentUser;

    public EstimateService(StandardInteriorsDbContext db, ICalculationService calculator, ISettingsService settings, ICurrentUserService currentUser)
    {
        _db = db;
        _calculator = calculator;
        _settings = settings;
        _currentUser = currentUser;
    }

    public async Task<PagedResult<EstimateListItemDto>> GetEstimatesAsync(PagingQuery query, int? customerId = null, EstimateStatus? status = null, DateTime? from = null, DateTime? to = null, CancellationToken cancellationToken = default)
    {
        var listQuery = _db.Estimates.AsNoTracking().Include(x => x.Customer).AsQueryable();

        if (customerId.HasValue)
        {
            listQuery = listQuery.Where(x => x.CustomerId == customerId.Value);
        }

        if (status.HasValue)
        {
            listQuery = listQuery.Where(x => x.Status == status.Value);
        }

        if (from.HasValue)
        {
            listQuery = listQuery.Where(x => x.EstimateDate >= from.Value);
        }

        if (to.HasValue)
        {
            listQuery = listQuery.Where(x => x.EstimateDate <= to.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim();
            listQuery = listQuery.Where(x => x.EstimateNumber.Contains(search) || x.Customer.Name.Contains(search) || x.Customer.Phone.Contains(search));
        }

        var projected = listQuery.Select(x => new
        {
            x.Id,
            x.EstimateNumber,
            x.CustomerId,
            CustomerName = x.Customer.Name,
            CustomerPhone = x.Customer.Phone,
            x.EstimateDate,
            SectionCount = x.Sections.Count,
            TotalAmount = x.CalculatedTotalAmount,
            ReceivedAmount = x.CalculatedReceivedAmount,
            Balance = x.CalculatedBalance,
            x.Status,
            x.PricingMode
        });

        projected = query.SortBy?.ToLowerInvariant() switch
        {
            "date" => query.SortDesc ? projected.OrderByDescending(x => x.EstimateDate) : projected.OrderBy(x => x.EstimateDate),
            "amount" => query.SortDesc ? projected.OrderByDescending(x => x.TotalAmount) : projected.OrderBy(x => x.TotalAmount),
            "balance" => query.SortDesc ? projected.OrderByDescending(x => x.Balance) : projected.OrderBy(x => x.Balance),
            _ => projected.OrderByDescending(x => x.EstimateDate)
        };

        var totalCount = await projected.CountAsync(cancellationToken);
        var page = Math.Max(query.Page, 1);
        var pageSize = Math.Clamp(query.PageSize, 1, 200);
        var rows = await projected.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
        var items = rows.Select(x => new EstimateListItemDto(x.Id, x.EstimateNumber, x.CustomerId, x.CustomerName, x.CustomerPhone, x.EstimateDate, x.SectionCount, x.TotalAmount, x.ReceivedAmount, x.Balance, x.Status, x.PricingMode)).ToList();
        return new PagedResult<EstimateListItemDto>(items, totalCount, page, pageSize);
    }

    public async Task<EstimateDetailDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var estimate = await _db.Estimates.AsNoTracking()
            .Include(x => x.Customer)
            .Include(x => x.Sections).ThenInclude(x => x.Items).ThenInclude(x => x.Service)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (estimate == null)
        {
            return null;
        }

        return MapDetail(estimate);
    }

    public async Task<int> CreateAsync(EstimateUpsertDto dto, CancellationToken cancellationToken = default)
    {
        ValidateEstimate(dto);
        var settings = await _settings.GetAsync(cancellationToken);
        var estimate = new Estimate
        {
            EstimateNumber = await GenerateEstimateNumberAsync(settings?.EstimatePrefix ?? "EST", cancellationToken),
            CustomerId = dto.CustomerId,
            EstimateDate = dto.EstimateDate,
            ValidUntil = dto.ValidUntil,
            PricingMode = dto.PricingMode,
            ManualOverallAmount = dto.ManualOverallAmount,
            Status = dto.Status,
            Notes = dto.Notes?.Trim(),
            EstimateNote = dto.EstimateNote?.Trim(),
            RevisionNumber = 1,
            Sections = MapSections(dto.Sections),
            CreatedByUserId = _currentUser.UserId,
            UpdatedByUserId = _currentUser.UserId
        };

        _db.Estimates.Add(estimate);
        await _db.SaveChangesAsync(cancellationToken);
        await RecalculateAsync(estimate.Id, cancellationToken);
        await ChangeStatusInternalAsync(estimate, null, estimate.Status, cancellationToken);
        return estimate.Id;
    }

    public async Task<bool> UpdateAsync(int id, EstimateUpsertDto dto, CancellationToken cancellationToken = default)
    {
        ValidateEstimate(dto);
        var estimate = await _db.Estimates.Include(x => x.Sections).ThenInclude(x => x.Items).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (estimate == null) return false;

        var previousStatus = estimate.Status;
        estimate.CustomerId = dto.CustomerId;
        estimate.EstimateDate = dto.EstimateDate;
        estimate.ValidUntil = dto.ValidUntil;
        estimate.PricingMode = dto.PricingMode;
        estimate.ManualOverallAmount = dto.ManualOverallAmount;
        estimate.Status = dto.Status;
        estimate.Notes = dto.Notes?.Trim();
        estimate.EstimateNote = dto.EstimateNote?.Trim();
        estimate.UpdatedAt = DateTime.UtcNow;
        estimate.UpdatedByUserId = _currentUser.UserId;

        _db.EstimateItems.RemoveRange(estimate.Sections.SelectMany(x => x.Items));
        _db.EstimateSections.RemoveRange(estimate.Sections);
        estimate.Sections = MapSections(dto.Sections);

        await _db.SaveChangesAsync(cancellationToken);
        await RecalculateAsync(estimate.Id, cancellationToken);
        await ChangeStatusInternalAsync(estimate, previousStatus, estimate.Status, cancellationToken);
        return true;
    }

    public async Task<int?> DuplicateAsync(int id, CancellationToken cancellationToken = default)
    {
        var existing = await _db.Estimates.Include(x => x.Sections).ThenInclude(x => x.Items).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (existing == null) return null;

        var copy = new Estimate
        {
            EstimateNumber = await GenerateEstimateNumberAsync((await _settings.GetAsync(cancellationToken))?.EstimatePrefix ?? "EST", cancellationToken),
            CustomerId = existing.CustomerId,
            EstimateDate = DateTime.Today,
            ValidUntil = existing.ValidUntil,
            PricingMode = existing.PricingMode,
            ManualOverallAmount = existing.ManualOverallAmount,
            Status = EstimateStatus.Draft,
            Notes = existing.Notes,
            EstimateNote = existing.EstimateNote,
            RevisionNumber = existing.RevisionNumber + 1,
            RevisedFromEstimateId = existing.Id,
            CreatedByUserId = _currentUser.UserId,
            UpdatedByUserId = _currentUser.UserId,
            Sections = existing.Sections.OrderBy(x => x.DisplayOrder).Select(CloneSection).ToList()
        };

        _db.Estimates.Add(copy);
        await _db.SaveChangesAsync(cancellationToken);
        await RecalculateAsync(copy.Id, cancellationToken);
        return copy.Id;
    }

    public async Task<bool> ChangeStatusAsync(int id, EstimateStatusChangeDto dto, CancellationToken cancellationToken = default)
    {
        var estimate = await _db.Estimates.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (estimate == null) return false;
        var previous = estimate.Status;
        estimate.Status = dto.Status;
        estimate.UpdatedAt = DateTime.UtcNow;
        _db.EstimateStatusHistory.Add(new EstimateStatusHistory
        {
            EstimateId = estimate.Id,
            FromStatus = previous,
            ToStatus = dto.Status,
            Notes = dto.Notes
        });
        await _db.SaveChangesAsync(cancellationToken);
        await RecalculateAsync(estimate.Id, cancellationToken);
        return true;
    }

    public async Task<bool> RecalculateAsync(int estimateId, CancellationToken cancellationToken = default)
    {
        var estimate = await _db.Estimates.Include(x => x.Sections).ThenInclude(x => x.Items).FirstOrDefaultAsync(x => x.Id == estimateId, cancellationToken);
        if (estimate == null) return false;

        var calc = _calculator.CalculateEstimate(estimate);
        estimate.CalculatedTotalAmount = calc.EstimatedTotal;

        var received = await _db.Payments.AsNoTracking().Where(x => x.EstimateId == estimateId && x.Status == PaymentStatus.Posted).SumAsync(x => (decimal?)x.Amount, cancellationToken) ?? 0m;
        estimate.CalculatedReceivedAmount = received;
        estimate.CalculatedBalance = _calculator.CalculateOutstanding(calc.EstimatedTotal, received);
        estimate.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static void ValidateEstimate(EstimateUpsertDto dto)
    {
        if (dto.CustomerId <= 0) throw new ArgumentException("Customer is required.");
        if (dto.EstimateDate == default) throw new ArgumentException("Estimate date is required.");
        if (dto.Sections.Count == 0) throw new ArgumentException("At least one section is required.");
    }

    private static List<EstimateSection> MapSections(IEnumerable<EstimateSectionDto> sections)
    {
        return sections.OrderBy(x => x.DisplayOrder).Select(section => new EstimateSection
        {
            SectionNumber = section.SectionNumber,
            Title = section.Title.Trim(),
            Description = section.Description?.Trim(),
            PricingMode = section.PricingMode,
            ManualSectionAmount = section.ManualSectionAmount,
            DisplayOrder = section.DisplayOrder,
            Items = section.Items.OrderBy(x => x.DisplayOrder).Select(item => new EstimateItem
            {
                ServiceId = item.ServiceId,
                Description = item.Description.Trim(),
                Quantity = item.Quantity,
                Unit = item.Unit?.Trim(),
                Rate = item.Rate,
                ManualAmount = item.ManualAmount,
                Notes = item.Notes?.Trim(),
                DisplayOrder = item.DisplayOrder,
                IsCustomItem = item.IsCustomItem
            }).ToList()
        }).ToList();
    }

    private static EstimateSection CloneSection(EstimateSection section)
        => new()
        {
            SectionNumber = section.SectionNumber,
            Title = section.Title,
            Description = section.Description,
            PricingMode = section.PricingMode,
            ManualSectionAmount = section.ManualSectionAmount,
            DisplayOrder = section.DisplayOrder,
            Items = section.Items.OrderBy(x => x.DisplayOrder).Select(item => new EstimateItem
            {
                ServiceId = item.ServiceId,
                Description = item.Description,
                Quantity = item.Quantity,
                Unit = item.Unit,
                Rate = item.Rate,
                ManualAmount = item.ManualAmount,
                IsCustomItem = item.IsCustomItem,
                Notes = item.Notes,
                DisplayOrder = item.DisplayOrder
            }).ToList()
        };

    private EstimateDetailDto MapDetail(Estimate estimate)
    {
        var customer = new CustomerSummaryLiteDto(estimate.Customer.Id, estimate.Customer.CustomerCode, estimate.Customer.Name, estimate.Customer.Phone, estimate.Customer.Location, estimate.Customer.Address);
        var sections = estimate.Sections.OrderBy(x => x.DisplayOrder).Select(section => new EstimateSectionDto
        {
            Id = section.Id,
            SectionNumber = section.SectionNumber,
            Title = section.Title,
            Description = section.Description,
            PricingMode = section.PricingMode,
            ManualSectionAmount = section.ManualSectionAmount,
            DisplayOrder = section.DisplayOrder,
            Items = section.Items.OrderBy(x => x.DisplayOrder).Select(item => new EstimateItemDto
            {
                Id = item.Id,
                ServiceId = item.ServiceId,
                ServiceName = item.Service?.ServiceName,
                Description = item.Description,
                Quantity = item.Quantity,
                Unit = item.Unit,
                Rate = item.Rate,
                ManualAmount = item.ManualAmount,
                Notes = item.Notes,
                DisplayOrder = item.DisplayOrder,
                IsCustomItem = item.IsCustomItem
            }).ToList()
        }).ToList();

        return new EstimateDetailDto(estimate.Id, estimate.EstimateNumber, customer, estimate.EstimateDate, estimate.ValidUntil, estimate.PricingMode, estimate.ManualOverallAmount, estimate.Status, estimate.Notes, estimate.EstimateNote, estimate.RevisionNumber, sections, estimate.CalculatedTotalAmount, estimate.CalculatedReceivedAmount, estimate.CalculatedBalance);
    }

    private async Task<string> GenerateEstimateNumberAsync(string prefix, CancellationToken cancellationToken)
    {
        var year = DateTime.Now.Year;
        var next = await _db.Estimates.CountAsync(cancellationToken) + 1;
        return $"{prefix}-{year}-{next:00000}";
    }

    private async Task ChangeStatusInternalAsync(Estimate estimate, EstimateStatus? fromStatus, EstimateStatus toStatus, CancellationToken cancellationToken)
    {
        if (fromStatus.HasValue && fromStatus.Value != toStatus)
        {
            _db.EstimateStatusHistory.Add(new EstimateStatusHistory
            {
                EstimateId = estimate.Id,
                FromStatus = fromStatus.Value,
                ToStatus = toStatus,
                Notes = null
            });
            await _db.SaveChangesAsync(cancellationToken);
        }
    }
}
