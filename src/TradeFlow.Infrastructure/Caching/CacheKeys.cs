namespace TradeFlow.Infrastructure.Caching;

public static class CacheKeys
{
    public static string OrderBook(string symbol) => $"tradeflow:order-book:{symbol.ToUpperInvariant()}";
    public static string RecentTrades(string symbol) => $"tradeflow:trades:{symbol.ToUpperInvariant()}";
}
