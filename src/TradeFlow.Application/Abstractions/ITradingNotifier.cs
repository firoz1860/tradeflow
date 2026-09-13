using TradeFlow.Domain.Entities;
using TradeFlow.MatchingEngine.Models;

namespace TradeFlow.Application.Abstractions;

public interface ITradingNotifier
{
    Task PublishOrderBookAsync(OrderBookSnapshot snapshot, CancellationToken cancellationToken);
    Task PublishTradesAsync(IReadOnlyList<Trade> trades, CancellationToken cancellationToken);
    Task PublishPortfolioAsync(string userId, IReadOnlyList<Wallet> wallets, CancellationToken cancellationToken);
    Task PublishRiskAlertAsync(string userId, string symbol, string reason, CancellationToken cancellationToken);
}
