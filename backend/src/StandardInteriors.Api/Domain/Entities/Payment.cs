using StandardInteriors.Api.Domain.Enums;

namespace StandardInteriors.Api.Domain.Entities;

public sealed class Payment : AuditableEntity
{
    public int Id { get; set; }
    public string PaymentNumber { get; set; } = string.Empty;
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = default!;
    public int? EstimateId { get; set; }
    public Estimate? Estimate { get; set; }
    public DateTime PaymentDate { get; set; }
    public decimal Amount { get; set; }
    public int PaymentMethodId { get; set; }
    public PaymentMethod PaymentMethod { get; set; } = default!;
    public string? TransactionReference { get; set; }
    public string? Notes { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Posted;
    public DateTime? VoidedAt { get; set; }
    public string? VoidReason { get; set; }
    public int? CreatedByUserId { get; set; }
    public int? UpdatedByUserId { get; set; }
    public int? VoidedByUserId { get; set; }
}
