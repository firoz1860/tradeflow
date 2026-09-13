using TradeFlow.Domain.Entities;

namespace TradeFlow.Contracts.Responses;

public sealed record TradeResponse(Guid Id, string Symbol, decimal Price, decimal Quantity, DateTimeOffset ExecutedAt)
{
    public static TradeResponse From(Trade trade) => new(trade.Id, trade.Symbol, trade.Price, trade.Quantity, trade.ExecutedAt);
}
