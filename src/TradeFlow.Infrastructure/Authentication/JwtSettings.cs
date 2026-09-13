namespace TradeFlow.Infrastructure.Authentication;

public sealed class JwtSettings
{
    public const string SectionName = "Jwt";
    public string Issuer { get; init; } = "TradeFlow";
    public string Audience { get; init; } = "TradeFlowDashboard";
    public string SigningKey { get; init; } = string.Empty;
    public int ExpiryMinutes { get; init; } = 120;
}
