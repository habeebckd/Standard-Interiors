using StandardInteriors.Api.Domain.Enums;

namespace StandardInteriors.Api.Dtos.Dashboard;

public sealed record MonthlyTrendPointDto(string Month, decimal AmountReceived);

public sealed record RecentEstimateDto(string EstimateNumber, string Customer, DateTime Date, decimal Amount, string Status, decimal Balance);

public sealed record RecentPaymentDto(DateTime Date, string Customer, decimal Amount, string Method, string? Reference);

public sealed record OutstandingCustomerDto(string Customer, decimal EstimateValue, decimal Received, decimal Balance);
public sealed record WorkStatusCountDto(WorkStatus Status, int Count);

public sealed record DashboardDto(
    decimal TotalCustomers,
    decimal TotalEstimates,
    decimal TotalEstimatedValue,
    decimal TotalAmountReceived,
    decimal TotalOutstandingBalance,
    decimal ThisMonthReceived,
    IReadOnlyList<MonthlyTrendPointDto> MonthlyPaymentTrend,
    IReadOnlyList<RecentEstimateDto> RecentEstimates,
    IReadOnlyList<RecentPaymentDto> RecentPayments,
    IReadOnlyList<OutstandingCustomerDto> OutstandingCustomers,
    IReadOnlyList<WorkStatusCountDto> WorkStatusSummary);
