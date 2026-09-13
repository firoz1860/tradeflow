using TradeFlow.Domain.Common;
using TradeFlow.Domain.Entities;
using TradeFlow.Domain.Enums;
using TradeFlow.MatchingEngine.Models;

namespace TradeFlow.MatchingEngine.Services;

/// <summary>Thread-safe, price-time-priority order book for exactly one trading symbol.</summary>
public sealed class MatchingEngineService
{
    private readonly object _gate = new();
    private readonly SortedDictionary<decimal, LinkedList<Order>> _bids = new(Comparer<decimal>.Create((left, right) => right.CompareTo(left)));
    private readonly SortedDictionary<decimal, LinkedList<Order>> _asks = new();
    private readonly Dictionary<Guid, BookEntry> _activeOrders = [];

    public MatchingEngineService(string symbol)
    {
        Symbol = symbol.ToUpperInvariant();
    }

    public string Symbol { get; }

    public ExecutionReport Process(Order incomingOrder)
    {
        if (incomingOrder.Symbol != Symbol) throw new DomainException($"Order symbol {incomingOrder.Symbol} does not match book {Symbol}.");

        lock (_gate)
        {
            var report = new ExecutionReport(incomingOrder);
            while (incomingOrder.RemainingQuantity > 0 && HasEligibleMaker(incomingOrder))
            {
                var makerBook = incomingOrder.Side == OrderSide.Buy ? _asks : _bids;
                var makerLevel = makerBook.First();
                var maker = makerLevel.Value.First!.Value;
                var fillQuantity = Math.Min(incomingOrder.RemainingQuantity, maker.RemainingQuantity);
                var trade = new Trade(maker, incomingOrder, makerLevel.Key, fillQuantity);

                maker.ApplyFill(fillQuantity);
                incomingOrder.ApplyFill(fillQuantity);
                report.Trades.Add(trade);
                report.AffectedOrders.Add(maker);
                report.AffectedOrders.Add(incomingOrder);

                if (maker.Status == OrderStatus.Filled) RemoveFromBook(maker.Id);
            }

            if (incomingOrder.RemainingQuantity > 0)
            {
                if (incomingOrder.Type == OrderType.Limit && incomingOrder.TimeInForce == TimeInForce.GoodTillCancelled)
                {
                    AddToBook(incomingOrder);
                }
                else
                {
                    incomingOrder.Cancel();
                    report.AffectedOrders.Add(incomingOrder);
                }
            }

            return report;
        }
    }

    public bool Cancel(Guid orderId, string userId)
    {
        lock (_gate)
        {
            if (!_activeOrders.TryGetValue(orderId, out var entry) || entry.Order.UserId != userId) return false;
            entry.Order.Cancel();
            RemoveFromBook(orderId);
            return true;
        }
    }

    public OrderBookSnapshot GetSnapshot(int depth = 10)
    {
        lock (_gate)
        {
            return new OrderBookSnapshot(
                Symbol,
                ToLevels(_bids, depth),
                ToLevels(_asks, depth),
                DateTimeOffset.UtcNow);
        }
    }

    private bool HasEligibleMaker(Order incomingOrder)
    {
        var opposite = incomingOrder.Side == OrderSide.Buy ? _asks : _bids;
        if (opposite.Count == 0) return false;
        if (incomingOrder.Type == OrderType.Market) return true;

        var bestOppositePrice = opposite.First().Key;
        return incomingOrder.Side == OrderSide.Buy
            ? incomingOrder.Price!.Value >= bestOppositePrice
            : incomingOrder.Price!.Value <= bestOppositePrice;
    }

    private void AddToBook(Order order)
    {
        var book = order.Side == OrderSide.Buy ? _bids : _asks;
        var price = order.Price!.Value;
        if (!book.TryGetValue(price, out var orders))
        {
            orders = [];
            book.Add(price, orders);
        }

        var node = orders.AddLast(order);
        _activeOrders.Add(order.Id, new BookEntry(order, price, order.Side, node));
        order.Rest();
    }

    private void RemoveFromBook(Guid orderId)
    {
        if (!_activeOrders.Remove(orderId, out var entry)) return;
        var book = entry.Side == OrderSide.Buy ? _bids : _asks;
        var level = entry.Node.List!;
        level.Remove(entry.Node);
        if (level.Count == 0) book.Remove(entry.Price);
    }

    private static IReadOnlyList<OrderBookLevel> ToLevels(SortedDictionary<decimal, LinkedList<Order>> book, int depth) =>
        book.Take(Math.Max(1, depth))
            .Select(level => new OrderBookLevel(level.Key, level.Value.Sum(order => order.RemainingQuantity), level.Value.Count))
            .ToList();

    private sealed record BookEntry(Order Order, decimal Price, OrderSide Side, LinkedListNode<Order> Node);
}
