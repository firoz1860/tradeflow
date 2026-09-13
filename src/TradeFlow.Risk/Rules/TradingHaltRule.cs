using TradeFlow.Domain.Entities;
using TradeFlow.Risk.Models;

namespace TradeFlow.Risk.Rules;

public sealed class TradingHaltRule : IRiskRule
{
    public string? Evaluate(Order order, RiskContext context) => context.isTradingHalted ? "Trading is currently halted for this pair." : null;
}
