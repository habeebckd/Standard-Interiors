using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using StandardInteriors.Api.Data;
using StandardInteriors.Api.Domain.Enums;
using StandardInteriors.Api.Dtos.Estimates;

namespace StandardInteriors.Api.Services.Implementations;

public interface IWorkStatusService
{
    Task<IReadOnlyList<WorkProjectDto>> GetProjectsAsync(int customerId, CancellationToken cancellationToken = default);
    Task<WorkProjectDto?> GetAsync(int estimateId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<WorkStatusHistoryDto>> GetHistoryAsync(int estimateId, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(int estimateId, WorkStatusUpdateDto dto, CancellationToken cancellationToken = default);
    Task<bool> StartAsync(int estimateId, WorkStatusUpdateDto dto, CancellationToken cancellationToken = default);
    Task<bool> ResumeAsync(int estimateId, WorkStatusUpdateDto dto, CancellationToken cancellationToken = default);
}

public sealed class WorkStatusService : IWorkStatusService
{
    private readonly StandardInteriorsDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public WorkStatusService(StandardInteriorsDbContext db, ICurrentUserService currentUser) { _db = db; _currentUser = currentUser; }

    public async Task<IReadOnlyList<WorkProjectDto>> GetProjectsAsync(int customerId, CancellationToken cancellationToken = default)
        => await _db.Estimates.AsNoTracking().Where(x => x.CustomerId == customerId).OrderByDescending(x => x.EstimateDate).Select(ProjectSelector()).ToListAsync(cancellationToken);

    public async Task<WorkProjectDto?> GetAsync(int estimateId, CancellationToken cancellationToken = default)
        => await _db.Estimates.AsNoTracking().Where(x => x.Id == estimateId).Select(ProjectSelector()).FirstOrDefaultAsync(cancellationToken);

    public async Task<IReadOnlyList<WorkStatusHistoryDto>> GetHistoryAsync(int estimateId, CancellationToken cancellationToken = default)
        => await _db.WorkStatusHistories.AsNoTracking().Where(x => x.EstimateId == estimateId).OrderBy(x => x.ChangedAt).Select(x => new WorkStatusHistoryDto(x.Id, x.PreviousStatus, x.NewStatus, x.ProgressPercentage, x.ChangedAt, x.ChangedByUser == null ? null : x.ChangedByUser.DisplayName, x.Description, x.StopReason, x.StopDate, x.ResumeDate)).ToListAsync(cancellationToken);

    public Task<bool> StartAsync(int estimateId, WorkStatusUpdateDto dto, CancellationToken cancellationToken = default)
        => UpdateAsync(estimateId, dto with { Status = WorkStatus.InProgress, ProgressPercentage = Math.Clamp(dto.ProgressPercentage == 0 ? 1 : dto.ProgressPercentage, 1, 99), EffectiveDate = dto.EffectiveDate ?? DateTime.UtcNow }, cancellationToken);

    public Task<bool> ResumeAsync(int estimateId, WorkStatusUpdateDto dto, CancellationToken cancellationToken = default)
        => UpdateAsync(estimateId, dto with { Status = WorkStatus.InProgress, ProgressPercentage = Math.Clamp(dto.ProgressPercentage == 0 ? 1 : dto.ProgressPercentage, 1, 99), EffectiveDate = dto.EffectiveDate ?? DateTime.UtcNow, ResumeNote = dto.ResumeNote ?? dto.Description }, cancellationToken);

    public async Task<bool> UpdateAsync(int estimateId, WorkStatusUpdateDto dto, CancellationToken cancellationToken = default)
    {
        var estimate = await _db.Estimates.FirstOrDefaultAsync(x => x.Id == estimateId, cancellationToken);
        if (estimate == null) return false;
        Validate(estimate.WorkStatus, dto);
        var effective = dto.EffectiveDate?.Date ?? DateTime.UtcNow.Date;
        var previous = estimate.WorkStatus;
        var progress = dto.Status == WorkStatus.Completed ? 100 : dto.Status == WorkStatus.NotStarted || dto.Status == WorkStatus.Scheduled ? 0 : dto.ProgressPercentage;
        estimate.WorkStatus = dto.Status;
        estimate.ProgressPercentage = progress;
        estimate.WorkStatusDescription = dto.Description?.Trim();
        estimate.PlannedStartDate = dto.PlannedStartDate?.Date ?? estimate.PlannedStartDate;
        estimate.PlannedEndDate = dto.PlannedEndDate?.Date ?? estimate.PlannedEndDate;
        estimate.UpdatedByUserId = _currentUser.UserId;
        estimate.UpdatedAt = DateTime.UtcNow;
        if (dto.Status == WorkStatus.InProgress && estimate.ActualStartDate == null) estimate.ActualStartDate = effective;
        if (dto.Status == WorkStatus.Completed) estimate.ActualEndDate = effective;
        if (dto.Status is WorkStatus.TemporarilyStopped or WorkStatus.PermanentlyStopped) { estimate.StopDate = effective; estimate.StopReason = dto.Description?.Trim(); }
        if (dto.Status == WorkStatus.InProgress && previous == WorkStatus.TemporarilyStopped) { estimate.ResumeDate = effective; estimate.ResumeNote = dto.ResumeNote?.Trim(); }
        _db.WorkStatusHistories.Add(new Domain.Entities.WorkStatusHistory { EstimateId = estimate.Id, PreviousStatus = previous, NewStatus = dto.Status, ProgressPercentage = progress, ChangedAt = DateTime.UtcNow, ChangedByUserId = _currentUser.UserId, Description = dto.Description?.Trim(), StopReason = dto.Status is WorkStatus.TemporarilyStopped or WorkStatus.PermanentlyStopped ? dto.Description?.Trim() : null, StopDate = dto.Status is WorkStatus.TemporarilyStopped or WorkStatus.PermanentlyStopped ? effective : null, ResumeDate = dto.Status == WorkStatus.InProgress && previous == WorkStatus.TemporarilyStopped ? effective : null });
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static void Validate(WorkStatus current, WorkStatusUpdateDto dto)
    {
        if (dto.ProgressPercentage is < 0 or > 100) throw new ArgumentException("Progress must be between 0 and 100.");
        if (dto.Status == WorkStatus.PermanentlyStopped && !dto.ConfirmPermanentStop) throw new ArgumentException("Permanent stop confirmation is required.");
        if (dto.Status is WorkStatus.TemporarilyStopped or WorkStatus.PermanentlyStopped && (dto.EffectiveDate == null || string.IsNullOrWhiteSpace(dto.Description))) throw new ArgumentException("Stop date and reason are required.");
        if (dto.Status == WorkStatus.InProgress && dto.ProgressPercentage is 0 or 100) throw new ArgumentException("In Progress must have progress between 1 and 99 percent.");
        if (dto.Status == WorkStatus.Completed && current == WorkStatus.PermanentlyStopped) throw new ArgumentException("A permanently stopped project cannot be completed.");
        if (dto.Status == WorkStatus.InProgress && current == WorkStatus.Completed) throw new ArgumentException("A completed project requires an intentional Admin correction.");
        if (dto.Status == WorkStatus.InProgress && current == WorkStatus.PermanentlyStopped) throw new ArgumentException("A permanently stopped project cannot be resumed.");
    }

    private Expression<Func<Domain.Entities.Estimate, WorkProjectDto>> ProjectSelector()
        => x => new WorkProjectDto(x.Id, x.EstimateNumber, x.Customer.Name, x.Notes, x.WorkStatus, x.ProgressPercentage, x.PlannedStartDate, x.ActualStartDate, x.PlannedEndDate, x.ActualEndDate, x.WorkStatusDescription, x.StopDate, x.StopReason, x.ResumeDate, x.ResumeNote, x.UpdatedAt, x.UpdatedByUserId == null ? null : _db.Users.Where(user => user.Id == x.UpdatedByUserId).Select(user => user.DisplayName).FirstOrDefault());
}
