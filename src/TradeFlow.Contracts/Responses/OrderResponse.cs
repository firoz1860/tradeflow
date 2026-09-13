using TradeFlow.Domain.Entities;

namespace TradeFlow.Contracts.Responses;

public sealed record OrderResponse(Guid Id, string Symbol, string Side, string Type, decimal Quantity, decimal RemainingQuantity, decimal? Price, string Status, string? RejectionReason)
{
    public static OrderResponse From(Order order) => new(order.Id, order.Symbol, order.Side.ToString(), order.Type.ToString(), order.OriginalQuantity, order.RemainingQuantity, order.Price, order.Status.ToString(), order.RejectionReason);
}
