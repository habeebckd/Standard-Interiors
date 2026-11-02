using StandardInteriors.Api.Domain.Enums;

namespace StandardInteriors.Api.Dtos.Estimates;

public sealed record EstimateItemDto
{
    public int? Id { get; init; }
    public int? ServiceId { get; init; }
    public string? ServiceName { get; init; }
    public string Description { get; init; } = string.Empty;
    public decimal? Quantity { get; init; }
    public string? Unit { get; init; }
    public decimal? Rate { get; init; }
    public decimal? ManualAmount { get; init; }
    public string? Notes { get; init; }
    public int DisplayOrder { get; init; }
    public bool IsCustomItem { get; init; }
}

public sealed record EstimateSectionDto
{
    public int? Id { get; init; }
    public string SectionNumber { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public EstimatePricingMode? PricingMode { get; init; }
    public decimal? ManualSectionAmount { get; init; }
    public int DisplayOrder { get; init; }
    public List<EstimateItemDto> Items { get; init; } = new();
}

public sealed record EstimateUpsertDto
{
    public int CustomerId { get; init; }
    public DateTime EstimateDate { get; init; }
    public DateTime? ValidUntil { get; init; }
    public EstimatePricingMode PricingMode { get; init; } = EstimatePricingMode.SectionBased;
    public decimal? ManualOverallAmount { get; init; }
    public EstimateStatus Status { get; init; } = EstimateStatus.Draft;
    public string? Notes { get; init; }
    public string? EstimateNote { get; init; }
    public List<EstimateSectionDto> Sections { get; init; } = new();
}

public sealed record EstimateListItemDto(
    int Id,
    string EstimateNumber,
    int CustomerId,
    string CustomerName,
    string CustomerPhone,
    DateTime EstimateDate,
    int SectionCount,
    decimal TotalAmount,
    decimal ReceivedAmount,
    decimal Balance,
    EstimateStatus Status,
    EstimatePricingMode PricingMode);

public sealed record EstimateDetailDto(
    int Id,
    string EstimateNumber,
    CustomerSummaryLiteDto Customer,
    DateTime EstimateDate,
    DateTime? ValidUntil,
    EstimatePricingMode PricingMode,
    decimal? ManualOverallAmount,
    EstimateStatus Status,
    string? Notes,
    string? EstimateNote,
    int RevisionNumber,
    List<EstimateSectionDto> Sections,
    decimal TotalAmount,
    decimal ReceivedAmount,
    decimal Balance);

public sealed record EstimateStatusChangeDto(EstimateStatus Status, string? Notes = null);

public sealed record CustomerSummaryLiteDto(int Id, string CustomerCode, string Name, string Phone, string? Location, string? Address);
