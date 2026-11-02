namespace StandardInteriors.Api.Domain.Entities;

public sealed class EstimateItem
{
    public int Id { get; set; }
    public int EstimateSectionId { get; set; }
    public EstimateSection EstimateSection { get; set; } = default!;
    public int? ServiceId { get; set; }
    public ServiceMaster? Service { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal? Quantity { get; set; }
    public string? Unit { get; set; }
    public decimal? Rate { get; set; }
    public decimal? ManualAmount { get; set; }
    public bool IsCustomItem { get; set; }
    public string? Notes { get; set; }
    public int DisplayOrder { get; set; }
}
