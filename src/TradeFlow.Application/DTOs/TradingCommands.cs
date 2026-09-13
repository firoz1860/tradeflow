using TradeFlow.Domain.Enums;

namespace TradeFlow.Application.DTOs;

public sealed record PlaceOrderCommand(
    string UserId,
    string Symbol,
    OrderSide Side,
    OrderType Type,
    decimal Quantity,
    decimal? Price,
    decimal? ReferencePrice,
    TimeInForce TimeInForce);

public sealed record PlaceOrderResult(bool IsAccepted, string? RejectionReason, Guid OrderId, IReadOnlyList<Guid> TradeIds);
