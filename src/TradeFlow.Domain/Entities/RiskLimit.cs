namespace TradeFlow.Domain.Entities;

public sealed class RiskLimit
{
    private RiskLimit() { }

    public RiskLimit(string userId, decimal maxOrderQuantity = 10m, decimal maxOrderNotional = 50_000m)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        MaxOrderQuantity = maxOrderQuantity;
        MaxOrderNotional = maxOrderNotional;
    }

    public Guid Id { get; private set; }
    public string UserId { get; private set; } = string.Empty;
    public decimal MaxOrderQuantity { get; private set; }
    public decimal MaxOrderNotional { get; private set; }
}
