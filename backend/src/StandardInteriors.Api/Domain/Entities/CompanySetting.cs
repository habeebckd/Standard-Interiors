namespace StandardInteriors.Api.Domain.Entities;

public sealed class CompanySetting : AuditableEntity
{
    public int Id { get; set; }
    public string CompanyName { get; set; } = "STANDARD INTERIORS";
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string EstimatePrefix { get; set; } = "EST";
    public string PaymentPrefix { get; set; } = "PAY";
    public string Currency { get; set; } = "INR";
    public string DefaultEstimateNote { get; set; } = "This estimate is prepared for the listed interior works. Final cost may vary based on site measurements, material/brand selection, design changes and additional work approved during execution.";
    public string? LogoPath { get; set; }
}
