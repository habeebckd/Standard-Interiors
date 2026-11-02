namespace StandardInteriors.Api.Domain.Entities;

public sealed class ServiceMaster : AuditableEntity
{
    public int Id { get; set; }
    public string ServiceCode { get; set; } = string.Empty;
    public string ServiceName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? DefaultUnit { get; set; }
    public decimal? DefaultPrice { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<EstimateItem> EstimateItems { get; set; } = new List<EstimateItem>();
}
