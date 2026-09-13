using TradeFlow.Domain.Entities;
using TradeFlow.Domain.Enums;
using TradeFlow.Risk.Models;
using TradeFlow.Risk.Services;

namespace TradeFlow.Risk.Tests;

public sealed class RiskEngineTests
{
    [Fact]
    public void Rejects_buy_when_available_cash_is_less_than_order_notional()
    {
        var engine = new RiskEngineService();
        var order = Order.Limit("trader", "BTC-USD", OrderSide.Buy, 100m, 2m);
        var context = new RiskContext(availableCash: 150m, availableAssetQuantity: 0m, maxQuantity: 10m, maxNotional: 10_000m, isTradingHalted: false);

        var result = engine.Evaluate(order, context);

        Assert.False(result.IsAllowed);
        Assert.Equal("Insufficient USD balance.", result.Reason);
    }

    [Fact]
    public void Rejects_an_order_above_the_configured_quantity_limit()
    {
        var engine = new RiskEngineService();
        var order = Order.Limit("trader", "BTC-USD", OrderSide.Sell, 100m, 11m);
        var context = new RiskContext(0m, 20m, 10m, 10_000m, false);

        var result = engine.Evaluate(order, context);

        Assert.False(result.IsAllowed);
        Assert.Equal("Order quantity exceeds the configured limit.", result.Reason);
    }

    [Fact]
    public void Rejects_order_when_a_trading_pair_is_halted()
    {
        var engine = new RiskEngineService();
        var order = Order.Limit("trader", "BTC-USD", OrderSide.Buy, 100m, 1m);
        var context = new RiskContext(1_000m, 0m, 10m, 10_000m, true);

        var result = engine.Evaluate(order, context);

        Assert.False(result.IsAllowed);
        Assert.Equal("Trading is currently halted for this pair.", result.Reason);
    }
}
