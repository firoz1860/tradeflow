using TradeFlow.Domain.Entities;
using TradeFlow.Risk.Models;
using TradeFlow.Risk.Rules;

namespace TradeFlow.Risk.Services;

public sealed class RiskEngineService
{
    private readonly IReadOnlyList<IRiskRule> _rules;

    public RiskEngineService() : this([new TradingHaltRule(), new MaxOrderQuantityRule(), new MaxOrderNotionalRule(), new SufficientBalanceRule()]) { }

    public RiskEngineService(IReadOnlyList<IRiskRule> rules)
    {
        _rules = rules;
    }

    public RiskCheckResult Evaluate(Order order, RiskContext context)
    {
        foreach (var rule in _rules)
        {
            var reason = rule.Evaluate(order, context);
            if (reason is not null) return RiskCheckResult.Reject(reason);
        }

        return RiskCheckResult.Allow();
    }
}
