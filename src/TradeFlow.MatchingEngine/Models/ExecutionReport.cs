using TradeFlow.Domain.Entities;

namespace TradeFlow.MatchingEngine.Models;

public sealed class ExecutionReport
{
    public ExecutionReport(Order incomingOrder)
    {
        IncomingOrder = incomingOrder;
    }

    public Order IncomingOrder { get; }
    public List<Trade> Trades { get; } = [];
    public HashSet<Order> AffectedOrders { get; } = [];
}
