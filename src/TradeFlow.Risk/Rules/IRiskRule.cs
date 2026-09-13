using TradeFlow.Domain.Entities;
using TradeFlow.Risk.Models;

namespace TradeFlow.Risk.Rules;

public interface IRiskRule
{
    string? Evaluate(Order order, RiskContext context);
}
