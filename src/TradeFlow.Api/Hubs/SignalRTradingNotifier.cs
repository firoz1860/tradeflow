using Microsoft.AspNetCore.SignalR;
using TradeFlow.Application.Abstractions;
using TradeFlow.Api.Hubs;
using TradeFlow.Contracts.Responses;
using TradeFlow.Contracts.SignalR;
using TradeFlow.Domain.Entities;
using TradeFlow.MatchingEngine.Models;

namespace TradeFlow.Api.Hubs;

public sealed class SignalRTradingNotifier(IHubContext<TradingHub> hub) : ITradingNotifier
{
    public Task PublishOrderBookAsync(OrderBookSnapshot snapshot, CancellationToken cancellationToken) =>
        hub.Clients.Group($"symbol:{snapshot.Symbol}").SendAsync("orderBookUpdated", new OrderBookUpdate(snapshot), cancellationToken);

    public Task PublishTradesAsync(IReadOnlyList<Trade> trades, CancellationToken cancellationToken)
    {
        if (trades.Count == 0) return Task.CompletedTask;
        var updates = trades.Select(TradeResponse.From).ToList();
        return hub.Clients.Group($"symbol:{trades[0].Symbol}").SendAsync("tradesExecuted", new TradeUpdate(updates), cancellationToken);
    }

    public Task PublishPortfolioAsync(string userId, IReadOnlyList<Wallet> wallets, CancellationToken cancellationToken) =>
        hub.Clients.User(userId).SendAsync("portfolioUpdated", new PortfolioUpdate(wallets.Select(WalletResponse.From).ToList()), cancellationToken);

    public Task PublishRiskAlertAsync(string userId, string symbol, string reason, CancellationToken cancellationToken) =>
        hub.Clients.User(userId).SendAsync("riskAlert", new RiskAlertUpdate(symbol, reason), cancellationToken);
}
