namespace StandardInteriors.Api.Dtos.Reports;

public sealed record MonthlyCollectionReportRowDto(string Month, decimal TotalReceived, decimal TotalOutstanding);

public sealed record OutstandingReportRowDto(string CustomerCode, string CustomerName, string Phone, string? Location, decimal EstimatedValue, decimal Received, decimal Outstanding);

public sealed record AllCustomerReportRowDto(string CustomerCode, string CustomerName, string Phone, string? Location, int TotalEstimates, decimal EstimatedValue, decimal Received, decimal Outstanding);

public sealed record AllEstimateReportRowDto(string EstimateNumber, string CustomerName, DateTime EstimateDate, string Status, decimal TotalAmount, decimal Received, decimal Outstanding);

public sealed record AllPaymentReportRowDto(string PaymentNumber, DateTime PaymentDate, string CustomerName, string? EstimateNumber, decimal Amount, string PaymentMethod, string? Reference, string Status);
