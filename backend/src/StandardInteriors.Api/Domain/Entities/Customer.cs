namespace StandardInteriors.Api.Domain.Entities;

public sealed class Customer : AuditableEntity
{
    public int Id { get; set; }
    public string CustomerCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? AlternatePhone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? Location { get; set; }
    public string? Notes { get; set; }
    public bool IsActive { get; set; } = true;
    public int? CreatedByUserId { get; set; }
    public int? UpdatedByUserId { get; set; }

    public ICollection<Estimate> Estimates { get; set; } = new List<Estimate>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
