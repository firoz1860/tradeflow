using TradeFlow.Domain.Common;
using TradeFlow.Domain.Enums;

namespace TradeFlow.Domain.Entities;

public sealed class Order
{
    private Order() { }

    private Order(string userId, string symbol, OrderSide side, OrderType type, decimal? price, decimal quantity, decimal? referencePrice, TimeInForce timeInForce)
    {
        if (string.IsNullOrWhiteSpace(userId)) throw new DomainException("User id is required.");
        if (string.IsNullOrWhiteSpace(symbol)) throw new DomainException("Trading symbol is required.");
        if (quantity <= 0) throw new DomainException("Quantity must be greater than zero.");
        if (type == OrderType.Limit && (!price.HasValue || price <= 0)) throw new DomainException("Limit orders require a positive price.");

        Id = Guid.NewGuid();
        UserId = userId;
        Symbol = symbol.ToUpperInvariant();
        Side = side;
        Type = type;
        Price = price;
        ReferencePrice = referencePrice;
        OriginalQuantity = quantity;
        RemainingQuantity = quantity;
        TimeInForce = timeInForce;
        Status = OrderStatus.New;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public Guid Id { get; private set; }
    public string UserId { get; private set; } = string.Empty;
    public string Symbol { get; private set; } = string.Empty;
    public OrderSide Side { get; private set; }
    public OrderType Type { get; private set; }
    public TimeInForce TimeInForce { get; private set; }
    public decimal? Price { get; private set; }
    public decimal? ReferencePrice { get; private set; }
    public decimal OriginalQuantity { get; private set; }
    public decimal RemainingQuantity { get; private set; }
    public OrderStatus Status { get; private set; }
    public string? RejectionReason { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? UpdatedAt { get; private set; }

    public static Order Limit(string userId, string symbol, OrderSide side, decimal price, decimal quantity, TimeInForce timeInForce = TimeInForce.GoodTillCancelled) =>
        new(userId, symbol, side, OrderType.Limit, price, quantity, price, timeInForce);

    public static Order Market(string userId, string symbol, OrderSide side, decimal quantity, decimal? referencePrice = null) =>
        new(userId, symbol, side, OrderType.Market, null, quantity, referencePrice, TimeInForce.ImmediateOrCancel);

    public void Rest()
    {
        if (Status == OrderStatus.New) Status = OrderStatus.Open;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void ApplyFill(decimal quantity)
    {
        if (quantity <= 0 || quantity > RemainingQuantity) throw new DomainException("Invalid fill quantity.");
        RemainingQuantity -= quantity;
        Status = RemainingQuantity == 0 ? OrderStatus.Filled : OrderStatus.PartiallyFilled;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void Cancel()
    {
        if (Status is OrderStatus.Filled or OrderStatus.Rejected) return;
        Status = OrderStatus.Cancelled;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void Reject(string reason)
    {
        Status = OrderStatus.Rejected;
        RejectionReason = reason;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}
