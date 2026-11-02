using StandardInteriors.Api.Domain.Enums;

namespace StandardInteriors.Api.Domain.Entities;

public sealed class WorkStatusHistory
{
    public int Id { get; set; }
    public int EstimateId { get; set; }
    public Estimate Estimate { get; set; } = default!;
    public WorkStatus? PreviousStatus { get; set; }
    public WorkStatus NewStatus { get; set; }
    public int ProgressPercentage { get; set; }
    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
    public int? ChangedByUserId { get; set; }
    public AppUser? ChangedByUser { get; set; }
    public string? Description { get; set; }
    public string? StopReason { get; set; }
    public DateTime? StopDate { get; set; }
    public DateTime? ResumeDate { get; set; }
}
