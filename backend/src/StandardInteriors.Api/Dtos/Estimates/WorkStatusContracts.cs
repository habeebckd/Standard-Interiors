using System.ComponentModel.DataAnnotations;
using StandardInteriors.Api.Domain.Enums;

namespace StandardInteriors.Api.Dtos.Estimates;

public sealed record WorkStatusUpdateDto
{
    public WorkStatus Status { get; init; }
    [Range(0, 100)] public int ProgressPercentage { get; init; }
    public DateTime? EffectiveDate { get; init; }
    public DateTime? PlannedStartDate { get; init; }
    public DateTime? PlannedEndDate { get; init; }
    public string? Description { get; init; }
    public bool ConfirmPermanentStop { get; init; }
    public string? ResumeNote { get; init; }
}

public sealed record WorkProjectDto(
    int EstimateId,
    string EstimateNumber,
    string CustomerName,
    string? ProjectName,
    WorkStatus Status,
    int ProgressPercentage,
    DateTime? PlannedStartDate,
    DateTime? ActualStartDate,
    DateTime? PlannedEndDate,
    DateTime? ActualEndDate,
    string? Description,
    DateTime? StopDate,
    string? StopReason,
    DateTime? ResumeDate,
    string? ResumeNote,
    DateTime LastUpdatedAt,
    string? LastUpdatedBy);

public sealed record WorkStatusHistoryDto(
    int Id,
    WorkStatus? PreviousStatus,
    WorkStatus NewStatus,
    int ProgressPercentage,
    DateTime ChangedAt,
    string? ChangedBy,
    string? Description,
    string? StopReason,
    DateTime? StopDate,
    DateTime? ResumeDate);
