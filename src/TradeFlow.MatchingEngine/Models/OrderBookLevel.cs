namespace TradeFlow.MatchingEngine.Models;

public sealed record OrderBookLevel(decimal Price, decimal Quantity, int OrderCount);
