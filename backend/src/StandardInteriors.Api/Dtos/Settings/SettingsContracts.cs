namespace StandardInteriors.Api.Dtos.Settings;

public sealed record CompanySettingsDto
{
    public int Id { get; init; }
    public string CompanyName { get; init; } = "STANDARD INTERIORS";
    public string? Address { get; init; }
    public string? Phone { get; init; }
    public string? Email { get; init; }
    public string EstimatePrefix { get; init; } = "EST";
    public string PaymentPrefix { get; init; } = "PAY";
    public string Currency { get; init; } = "INR";
    public string DefaultEstimateNote { get; init; } = string.Empty;
    public string? LogoPath { get; init; }
}
