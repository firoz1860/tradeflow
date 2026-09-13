using TradeFlow.Domain.Entities;
using TradeFlow.Risk.Models;

namespace TradeFlow.Risk.Rules;

public sealed class MaxOrderQuantityRule : IRiskRule
{
    public string? Evaluate(Order order, RiskContext context) => order.OriginalQuantity > context.maxQuantity ? "Order quantity exceeds the configured limit." : null;
}
