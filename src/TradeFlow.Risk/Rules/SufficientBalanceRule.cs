using TradeFlow.Domain.Entities;
using TradeFlow.Domain.Enums;
using TradeFlow.Risk.Models;

namespace TradeFlow.Risk.Rules;

public sealed class SufficientBalanceRule : IRiskRule
{
    public string? Evaluate(Order order, RiskContext context)
    {
        if (order.Side == OrderSide.Sell)
        {
            return context.availableAssetQuantity < order.OriginalQuantity ? "Insufficient asset balance." : null;
        }

        var estimatedPrice = order.Price ?? order.ReferencePrice;
        if (!estimatedPrice.HasValue) return "Market buy orders require a reference price for risk checks.";
        return context.availableCash < estimatedPrice.Value * order.OriginalQuantity ? "Insufficient USD balance." : null;
    }
}
