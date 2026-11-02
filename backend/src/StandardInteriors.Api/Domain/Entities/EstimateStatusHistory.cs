using StandardInteriors.Api.Domain.Enums;

namespace StandardInteriors.Api.Domain.Entities;

public sealed class EstimateStatusHistory
{
    public int Id { get; set; }
    public int EstimateId { get; set; }
    public Estimate Estimate { get; set; } = default!;
    public EstimateStatus FromStatus { get; set; }
    public EstimateStatus ToStatus { get; set; }
    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
    public string? Notes { get; set; }
}
