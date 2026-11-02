using StandardInteriors.Api.Domain.Enums;

namespace StandardInteriors.Api.Domain.Entities;

public sealed class Estimate : AuditableEntity
{
    public int Id { get; set; }
    public string EstimateNumber { get; set; } = string.Empty;
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = default!;
    public DateTime EstimateDate { get; set; }
    public DateTime? ValidUntil { get; set; }
    public EstimatePricingMode PricingMode { get; set; } = EstimatePricingMode.SectionBased;
    public decimal? ManualOverallAmount { get; set; }
    public EstimateStatus Status { get; set; } = EstimateStatus.Draft;
    public WorkStatus WorkStatus { get; set; } = WorkStatus.NotStarted;
    public DateTime? PlannedStartDate { get; set; }
    public DateTime? ActualStartDate { get; set; }
    public DateTime? PlannedEndDate { get; set; }
    public DateTime? ActualEndDate { get; set; }
    public int ProgressPercentage { get; set; }
    public string? WorkStatusDescription { get; set; }
    public DateTime? StopDate { get; set; }
    public string? StopReason { get; set; }
    public DateTime? ResumeDate { get; set; }
    public string? ResumeNote { get; set; }
    public string? Notes { get; set; }
    public string? EstimateNote { get; set; }
    public int RevisionNumber { get; set; } = 1;
    public int? RevisedFromEstimateId { get; set; }
    public decimal CalculatedTotalAmount { get; set; }
    public decimal CalculatedReceivedAmount { get; set; }
    public decimal CalculatedBalance { get; set; }
    public int? CreatedByUserId { get; set; }
    public int? UpdatedByUserId { get; set; }

    public ICollection<EstimateSection> Sections { get; set; } = new List<EstimateSection>();
    public ICollection<EstimateStatusHistory> StatusHistory { get; set; } = new List<EstimateStatusHistory>();
    public ICollection<WorkStatusHistory> WorkStatusHistories { get; set; } = new List<WorkStatusHistory>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
