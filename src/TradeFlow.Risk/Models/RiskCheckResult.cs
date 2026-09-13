namespace TradeFlow.Risk.Models;

public sealed record RiskCheckResult(bool IsAllowed, string? Reason)
{
    public static RiskCheckResult Allow() => new(true, null);
    public static RiskCheckResult Reject(string reason) => new(false, reason);
}
