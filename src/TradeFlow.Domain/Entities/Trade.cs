using TradeFlow.Domain.Enums;

namespace TradeFlow.Domain.Entities;

public sealed class Trade
{
    private Trade() { }

    public Trade(Order maker, Order taker, decimal price, decimal quantity)
    {
        Id = Guid.NewGuid();
        Symbol = maker.Symbol;
        MakerOrderId = maker.Id;
        TakerOrderId = taker.Id;
        MakerUserId = maker.UserId;
        TakerUserId = taker.UserId;
        MakerSide = maker.Side;
        Price = price;
        Quantity = quantity;
        ExecutedAt = DateTimeOffset.UtcNow;
    }

    public Guid Id { get; private set; }
    public string Symbol { get; private set; } = string.Empty;
    public Guid MakerOrderId { get; private set; }
    public Guid TakerOrderId { get; private set; }
    public string MakerUserId { get; private set; } = string.Empty;
    public string TakerUserId { get; private set; } = string.Empty;
    public OrderSide MakerSide { get; private set; }
    public decimal Price { get; private set; }
    public decimal Quantity { get; private set; }
    public DateTimeOffset ExecutedAt { get; private set; }
}
