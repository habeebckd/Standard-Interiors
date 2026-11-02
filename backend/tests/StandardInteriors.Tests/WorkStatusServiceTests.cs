using Microsoft.EntityFrameworkCore;
using StandardInteriors.Api.Data;
using StandardInteriors.Api.Domain.Entities;
using StandardInteriors.Api.Domain.Enums;
using StandardInteriors.Api.Dtos.Estimates;
using StandardInteriors.Api.Services.Implementations;

namespace StandardInteriors.Tests;

public sealed class WorkStatusServiceTests
{
    [Fact]
    public async Task WorkLifecycle_PreservesHistoryAndCompletesAt100Percent()
    {
        await using var db = CreateDb();
        db.Customers.Add(new Customer { Id = 1, CustomerCode = "CUS-000001", Name = "Test Customer", Phone = "9000000000" });
        db.Estimates.Add(new Estimate { Id = 1, EstimateNumber = "EST-000001", CustomerId = 1, EstimateDate = DateTime.Today });
        await db.SaveChangesAsync();
        var service = new WorkStatusService(db, new TestCurrentUser(1));

        await service.StartAsync(1, new WorkStatusUpdateDto { EffectiveDate = new DateTime(2026, 8, 15), ProgressPercentage = 1, Description = "Started" });
        await service.UpdateAsync(1, new WorkStatusUpdateDto { Status = WorkStatus.InProgress, EffectiveDate = new DateTime(2026, 8, 25), ProgressPercentage = 40, Description = "Progress" });
        await service.UpdateAsync(1, new WorkStatusUpdateDto { Status = WorkStatus.TemporarilyStopped, EffectiveDate = new DateTime(2026, 8, 29), ProgressPercentage = 40, Description = "Waiting for material" });
        await service.ResumeAsync(1, new WorkStatusUpdateDto { EffectiveDate = new DateTime(2026, 9, 2), ProgressPercentage = 40, ResumeNote = "Resumed" });
        await service.UpdateAsync(1, new WorkStatusUpdateDto { Status = WorkStatus.Completed, EffectiveDate = new DateTime(2026, 9, 15), ProgressPercentage = 80, Description = "Completed" });

        var estimate = await db.Estimates.SingleAsync();
        Assert.Equal(WorkStatus.Completed, estimate.WorkStatus);
        Assert.Equal(100, estimate.ProgressPercentage);
        Assert.Equal(new DateTime(2026, 9, 15), estimate.ActualEndDate);
        Assert.Equal(5, await db.WorkStatusHistories.CountAsync());
    }

    [Fact]
    public async Task StoppedWorkRequiresReasonAndPermanentStopCannotResume()
    {
        await using var db = CreateDb();
        db.Estimates.Add(new Estimate { Id = 1, EstimateNumber = "EST-000001", Customer = new Customer { Id = 1, CustomerCode = "CUS-000001", Name = "Test", Phone = "9000000000" } });
        await db.SaveChangesAsync();
        var service = new WorkStatusService(db, new TestCurrentUser(1));

        await Assert.ThrowsAsync<ArgumentException>(() => service.UpdateAsync(1, new WorkStatusUpdateDto { Status = WorkStatus.TemporarilyStopped, ProgressPercentage = 20 }));
        await service.UpdateAsync(1, new WorkStatusUpdateDto { Status = WorkStatus.PermanentlyStopped, EffectiveDate = DateTime.Today, ProgressPercentage = 20, Description = "Closed", ConfirmPermanentStop = true });
        await Assert.ThrowsAsync<ArgumentException>(() => service.ResumeAsync(1, new WorkStatusUpdateDto { ProgressPercentage = 20, EffectiveDate = DateTime.Today }));
    }

    [Fact]
    public async Task InvalidProgressIsRejected()
    {
        await using var db = CreateDb();
        db.Estimates.Add(new Estimate { Id = 1, EstimateNumber = "EST-000001", Customer = new Customer { Id = 1, CustomerCode = "CUS-000001", Name = "Test", Phone = "9000000000" } });
        await db.SaveChangesAsync();
        var service = new WorkStatusService(db, new TestCurrentUser(1));

        await Assert.ThrowsAsync<ArgumentException>(() => service.UpdateAsync(1, new WorkStatusUpdateDto { Status = WorkStatus.InProgress, ProgressPercentage = 101 }));
    }

    private static StandardInteriorsDbContext CreateDb() => new(new DbContextOptionsBuilder<StandardInteriorsDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

    private sealed class TestCurrentUser(int id) : ICurrentUserService
    {
        public int? UserId => id;
        public string? Username => "test";
    }
}
