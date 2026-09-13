using TradeFlow.Contracts.Responses;
using TradeFlow.MatchingEngine.Models;

namespace TradeFlow.Contracts.SignalR;

public sealed record OrderBookUpdate(OrderBookSnapshot Snapshot);
public sealed record TradeUpdate(IReadOnlyList<TradeResponse> Trades);
public sealed record PortfolioUpdate(IReadOnlyList<WalletResponse> Wallets);
public sealed record RiskAlertUpdate(string Symbol, string Reason);
