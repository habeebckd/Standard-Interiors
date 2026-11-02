using StandardInteriors.Api.Domain.Enums;

namespace StandardInteriors.Api.Domain.Entities;

public sealed class EstimateSection
{
    public int Id { get; set; }
    public int EstimateId { get; set; }
    public Estimate Estimate { get; set; } = default!;
    public string SectionNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public EstimatePricingMode? PricingMode { get; set; }
    public decimal? ManualSectionAmount { get; set; }
    public int DisplayOrder { get; set; }

    public ICollection<EstimateItem> Items { get; set; } = new List<EstimateItem>();
}
