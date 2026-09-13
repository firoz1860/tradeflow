using TradeFlow.Domain.Entities;
using TradeFlow.MatchingEngine.Models;

namespace TradeFlow.MatchingEngine.Concurrency;

internal sealed record OrderCommand(Order Order, TaskCompletionSource<ExecutionReport> Completion);
