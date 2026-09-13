using TradeFlow.Domain.Entities;
using TradeFlow.Domain.Enums;
using TradeFlow.MatchingEngine.Services;

namespace TradeFlow.MatchingEngine.Tests;

public sealed class MatchingEngineTests
{
    [Fact]
    public void Matches_the_oldest_order_first_at_the_same_price()
    {
        var engine = new MatchingEngineService("BTC-USD");
        var first = Order.Limit("trader-a", "BTC-USD", OrderSide.Sell, 100m, 1m);
        var second = Order.Limit("trader-b", "BTC-USD", OrderSide.Sell, 100m, 1m);
        engine.Process(first);
        engine.Process(second);

        var result = engine.Process(Order.Limit("trader-c", "BTC-USD", OrderSide.Buy, 100m, 1m));

        Assert.Single(result.Trades);
        Assert.Equal(first.Id, result.Trades[0].MakerOrderId);
        Assert.Equal(OrderStatus.Filled, first.Status);
        Assert.Equal(OrderStatus.Open, second.Status);
    }

    [Fact]
    public void Leaves_unfilled_limit_quantity_on_the_book_after_a_partial_fill()
    {
        var engine = new MatchingEngineService("BTC-USD");
        engine.Process(Order.Limit("seller", "BTC-USD", OrderSide.Sell, 100m, 2m));

        var buy = Order.Limit("buyer", "BTC-USD", OrderSide.Buy, 100m, 5m);
        var result = engine.Process(buy);

        Assert.Single(result.Trades);
        Assert.Equal(3m, buy.RemainingQuantity);
        Assert.Equal(OrderStatus.PartiallyFilled, buy.Status);
        Assert.Equal(3m, engine.GetSnapshot().Bids.Single().Quantity);
    }

    [Fact]
    public void Cancels_an_open_order_by_its_id()
    {
        var engine = new MatchingEngineService("BTC-USD");
        var order = Order.Limit("buyer", "BTC-USD", OrderSide.Buy, 95m, 2m);
        engine.Process(order);

        var cancelled = engine.Cancel(order.Id, "buyer");

        Assert.True(cancelled);
        Assert.Equal(OrderStatus.Cancelled, order.Status);
        Assert.Empty(engine.GetSnapshot().Bids);
    }

    [Fact]
    public void Cancels_the_unfilled_remainder_of_a_market_order()
    {
        var engine = new MatchingEngineService("BTC-USD");
        engine.Process(Order.Limit("seller", "BTC-USD", OrderSide.Sell, 100m, 1m));
        var marketBuy = Order.Market("buyer", "BTC-USD", OrderSide.Buy, 3m);

        var result = engine.Process(marketBuy);

        Assert.Single(result.Trades);
        Assert.Equal(2m, marketBuy.RemainingQuantity);
        Assert.Equal(OrderStatus.Cancelled, marketBuy.Status);
    }
}
