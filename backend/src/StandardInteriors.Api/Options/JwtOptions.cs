namespace StandardInteriors.Api.Options;

public sealed class JwtOptions
{
    public string Key { get; set; } = string.Empty;
    public string Issuer { get; set; } = "StandardInteriors.Api";
    public string Audience { get; set; } = "StandardInteriors.Frontend";
    public int ExpirationMinutes { get; set; } = 480;
}
