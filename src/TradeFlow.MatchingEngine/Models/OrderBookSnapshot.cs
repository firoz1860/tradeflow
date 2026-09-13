namespace TradeFlow.MatchingEngine.Models;

public sealed record OrderBookSnapshot(string Symbol, IReadOnlyList<OrderBookLevel> Bids, IReadOnlyList<OrderBookLevel> Asks, DateTimeOffset GeneratedAt);
