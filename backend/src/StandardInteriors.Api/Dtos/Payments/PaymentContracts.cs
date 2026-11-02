using StandardInteriors.Api.Domain.Enums;

namespace StandardInteriors.Api.Dtos.Payments;

public sealed record PaymentUpsertDto
{
    public int CustomerId { get; init; }
    public int? EstimateId { get; init; }
    public DateTime PaymentDate { get; init; }
    public decimal Amount { get; init; }
    public int PaymentMethodId { get; init; }
    public string? TransactionReference { get; init; }
    public string? Notes { get; init; }
    public bool AllowOverpayment { get; init; }
}

public sealed record PaymentDto(
    int Id,
    string PaymentNumber,
    int CustomerId,
    string CustomerName,
    int? EstimateId,
    string? EstimateNumber,
    DateTime PaymentDate,
    decimal Amount,
    int PaymentMethodId,
    string PaymentMethodName,
    string? TransactionReference,
    string? Notes,
    PaymentStatus Status,
    DateTime? VoidedAt,
    string? VoidReason);

public sealed record PaymentListItemDto(
    int Id,
    string PaymentNumber,
    DateTime PaymentDate,
    string CustomerName,
    string? EstimateNumber,
    decimal Amount,
    string PaymentMethodName,
    string? TransactionReference,
    string? Notes,
    PaymentStatus Status);

public sealed record PaymentVoidDto(string Reason);

public sealed record PaymentReceiptDto(
    string ReceiptNumber,
    DateTime PaymentDate,
    string CustomerName,
    string? EstimateNumber,
    decimal PreviousBalance,
    decimal AmountReceived,
    decimal RemainingBalance,
    string PaymentMethod,
    string? TransactionReference,
    string? Note);

public sealed record PaymentSheetDto(
    string CustomerName,
    string? EstimateNumber,
    decimal TotalPrice,
    decimal TotalPaid,
    decimal RemainingBalance,
    IReadOnlyList<PaymentListItemDto> Payments);
