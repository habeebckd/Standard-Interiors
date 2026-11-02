using System.ComponentModel.DataAnnotations;
using StandardInteriors.Api.Domain.Enums;

namespace StandardInteriors.Api.Dtos.Customers;

public sealed record CustomerListItemDto(
    int Id,
    string CustomerCode,
    string Name,
    string Phone,
    string? AlternatePhone,
    string? Location,
    decimal TotalEstimateValue,
    decimal TotalReceived,
    decimal Outstanding,
    int EstimateCount,
    DateTime? LastActivity,
    WorkStatus? LatestWorkStatus,
    int? LatestWorkProgress);

public sealed record CustomerSummaryDto(
    int Id,
    string CustomerCode,
    string Name,
    string Phone,
    string? AlternatePhone,
    string? Email,
    string? Address,
    string? Location,
    string? Notes,
    decimal TotalEstimated,
    decimal TotalReceived,
    decimal Outstanding,
    int TotalEstimates,
    int TotalPayments);

public sealed record CustomerDetailDto(
    int Id,
    string CustomerCode,
    string Name,
    string Phone,
    string? AlternatePhone,
    string? Email,
    string? Address,
    string? Location,
    string? Notes,
    bool IsActive,
    CustomerSummaryDto Summary);

public sealed record CustomerUpsertDto
{
    [Required, StringLength(200)]
    public string Name { get; init; } = string.Empty;
    [Required, StringLength(50)]
    public string Phone { get; init; } = string.Empty;
    [StringLength(50)]
    public string? AlternatePhone { get; init; }
    [EmailAddress, StringLength(200)]
    public string? Email { get; init; }
    [StringLength(1000)]
    public string? Address { get; init; }
    [StringLength(200)]
    public string? Location { get; init; }
    [StringLength(2000)]
    public string? Notes { get; init; }
    public bool IsActive { get; init; } = true;
}

public sealed record CustomerStatementLineDto(
    DateTime Date,
    string Reference,
    string Type,
    decimal Debit,
    decimal Credit,
    decimal RunningBalance);

public sealed record CustomerStatementDto(
    CustomerSummaryDto Summary,
    IReadOnlyList<CustomerStatementLineDto> Lines);
