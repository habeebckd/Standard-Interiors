using StandardInteriors.Api.Domain.Enums;

namespace StandardInteriors.Api.Dtos.Reports;

public sealed class ReportFilter
{
    public DateTime? DateFrom { get; init; }
    public DateTime? DateTo { get; init; }
    public int? CustomerId { get; init; }
    public string? Customer { get; init; }
    public EstimateStatus? EstimateStatus { get; init; }
    public int? PaymentMethodId { get; init; }
    public bool? HasOutstanding { get; init; }
    public bool? FullyPaid { get; init; }
}
