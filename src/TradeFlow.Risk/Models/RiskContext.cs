namespace TradeFlow.Risk.Models;

public sealed record RiskContext(
    decimal availableCash,
    decimal availableAssetQuantity,
    decimal maxQuantity,
    decimal maxNotional,
    bool isTradingHalted);
