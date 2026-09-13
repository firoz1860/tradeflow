using TradeFlow.Domain.Entities;
using TradeFlow.Risk.Models;

namespace TradeFlow.Risk.Rules;

public sealed class MaxOrderNotionalRule : IRiskRule
{
    public string? Evaluate(Order order, RiskContext context)
    {
        var estimatedPrice = order.Price ?? order.ReferencePrice;
        return estimatedPrice.HasValue && estimatedPrice.Value * order.OriginalQuantity > context.maxNotional
            ? "Order value exceeds the configured limit."
            : null;
    }
}
