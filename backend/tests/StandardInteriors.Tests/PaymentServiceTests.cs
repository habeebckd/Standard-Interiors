using Microsoft.EntityFrameworkCore;
using StandardInteriors.Api.Data;
using StandardInteriors.Api.Domain.Entities;
using StandardInteriors.Api.Domain.Enums;
using StandardInteriors.Api.Dtos.Common;
using StandardInteriors.Api.Dtos.Customers;
using StandardInteriors.Api.Dtos.Estimates;
using StandardInteriors.Api.Dtos.Payments;
using StandardInteriors.Api.Services;
using StandardInteriors.Api.Services.Implementations;

namespace StandardInteriors.Tests;

public sealed class PaymentServiceTests
{
    [Fact]
    public async Task CreateAsync_RejectsInvalidAmount()
    {
        var options = new DbContextOptionsBuilder<StandardInteriorsDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        await using var db = new StandardInteriorsDbContext(options);
        db.Customers.Add(new Customer { Id = 1, CustomerCode = "CUS-000001", Name = "Test", Phone = "9999999999" });
        db.PaymentMethods.Add(new PaymentMethod { Id = 1, Code = "CASH", Name = "Cash" });
        await db.SaveChangesAsync();

        var service = new PaymentService(db, new CalculationService(), new FakeEstimateService(), new FakeCustomerService(), new FakeCurrentUserService());

        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateAsync(new PaymentUpsertDto
        {
            CustomerId = 1,
            PaymentDate = DateTime.Today,
            Amount = 0m,
            PaymentMethodId = 1
        }));
    }

    [Fact]
    public async Task VoidAsync_ExcludesVoidedPaymentFromTotals()
    {
        var options = new DbContextOptionsBuilder<StandardInteriorsDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        await using var db = new StandardInteriorsDbContext(options);
        db.Customers.Add(new Customer { Id = 1, CustomerCode = "CUS-000001", Name = "Test", Phone = "9999999999" });
        db.PaymentMethods.Add(new PaymentMethod { Id = 1, Code = "CASH", Name = "Cash" });
        db.Estimates.Add(new Estimate { Id = 1, CustomerId = 1, EstimateNumber = "EST-2026-00001", EstimateDate = DateTime.Today, PricingMode = EstimatePricingMode.Overall, ManualOverallAmount = 100000m, CalculatedTotalAmount = 100000m, CalculatedReceivedAmount = 0m, CalculatedBalance = 100000m, Status = EstimateStatus.Approved });
        db.Payments.Add(new Payment { Id = 1, PaymentNumber = "PAY-2026-00001", CustomerId = 1, EstimateId = 1, PaymentDate = DateTime.Today, Amount = 20000m, PaymentMethodId = 1, Status = PaymentStatus.Posted });
        await db.SaveChangesAsync();

        var estimateService = new FakeEstimateService();
        var customerService = new FakeCustomerService();
        var service = new PaymentService(db, new CalculationService(), estimateService, customerService, new FakeCurrentUserService());

        await service.VoidAsync(1, "Duplicate entry");

        var receipt = await service.GetReceiptAsync(1);
        Assert.NotNull(receipt);
        Assert.Equal(PaymentStatus.Voided, (await db.Payments.FirstAsync(x => x.Id == 1)).Status);
    }

    private sealed class FakeEstimateService : IEstimateService
    {
        public Task<bool> ChangeStatusAsync(int id, EstimateStatusChangeDto dto, CancellationToken cancellationToken = default) => Task.FromResult(true);
        public Task<int> CreateAsync(EstimateUpsertDto dto, CancellationToken cancellationToken = default) => Task.FromResult(1);
        public Task<int?> DuplicateAsync(int id, CancellationToken cancellationToken = default) => Task.FromResult<int?>(1);
        public Task<EstimateDetailDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default) => Task.FromResult<EstimateDetailDto?>(null);
        public Task<PagedResult<EstimateListItemDto>> GetEstimatesAsync(PagingQuery query, int? customerId = null, EstimateStatus? status = null, DateTime? from = null, DateTime? to = null, CancellationToken cancellationToken = default) => Task.FromResult(PagedResult<EstimateListItemDto>.Empty());
        public Task<bool> RecalculateAsync(int estimateId, CancellationToken cancellationToken = default) => Task.FromResult(true);
        public Task<bool> UpdateAsync(int id, EstimateUpsertDto dto, CancellationToken cancellationToken = default) => Task.FromResult(true);
    }

    private sealed class FakeCustomerService : ICustomerService
    {
        public Task<int> CreateAsync(CustomerUpsertDto dto, CancellationToken cancellationToken = default) => Task.FromResult(1);
        public Task<CustomerDetailDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default) => Task.FromResult<CustomerDetailDto?>(null);
        public Task<PagedResult<CustomerListItemDto>> GetCustomersAsync(PagingQuery query, bool? hasOutstanding, bool? fullyPaid, bool? hasEstimates, DateTime? from, DateTime? to, CancellationToken cancellationToken = default) => Task.FromResult(PagedResult<CustomerListItemDto>.Empty());
        public Task<CustomerStatementDto?> GetStatementAsync(int id, CancellationToken cancellationToken = default) => Task.FromResult<CustomerStatementDto?>(null);
        public Task<CustomerSummaryDto?> GetSummaryAsync(int id, CancellationToken cancellationToken = default) => Task.FromResult<CustomerSummaryDto?>(new CustomerSummaryDto(1, "CUS-000001", "Test", "9999999999", null, null, null, null, null, 100000m, 0m, 100000m, 1, 0));
        public Task<bool> UpdateAsync(int id, CustomerUpsertDto dto, CancellationToken cancellationToken = default) => Task.FromResult(true);
    }

    private sealed class FakeCurrentUserService : ICurrentUserService
    {
        public int? UserId => 1;
        public string? Username => "test";
    }
}
