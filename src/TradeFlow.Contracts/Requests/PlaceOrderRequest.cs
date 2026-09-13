using TradeFlow.Domain.Enums;

namespace TradeFlow.Contracts.Requests;

public sealed record PlaceOrderRequest(
    string Symbol,
    OrderSide Side,
    OrderType Type,
    decimal Quantity,
    decimal? Price,
    decimal? ReferencePrice = null,
    TimeInForce TimeInForce = TimeInForce.GoodTillCancelled);
