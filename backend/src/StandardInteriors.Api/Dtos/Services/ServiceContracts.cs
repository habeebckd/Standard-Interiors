namespace StandardInteriors.Api.Dtos.Services;

public sealed record ServiceDto(
    int Id,
    string ServiceCode,
    string ServiceName,
    string? Description,
    string? DefaultUnit,
    decimal? DefaultPrice,
    int DisplayOrder,
    bool IsActive);

public sealed record ServiceUpsertDto
{
    public string ServiceName { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? DefaultUnit { get; init; }
    public decimal? DefaultPrice { get; init; }
    public int DisplayOrder { get; init; }
    public bool IsActive { get; init; } = true;
}

public sealed record PaymentMethodDto(int Id, string Code, string Name, bool IsActive, int DisplayOrder);

public sealed record PaymentMethodUpsertDto
{
    public string Name { get; init; } = string.Empty;
    public bool IsActive { get; init; } = true;
    public int DisplayOrder { get; init; }
}
