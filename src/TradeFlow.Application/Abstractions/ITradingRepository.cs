using TradeFlow.Domain.Entities;

namespace TradeFlow.Application.Abstractions;

public interface ITradingRepository
{
    Task<TradingPair?> FindPairAsync(string symbol, CancellationToken cancellationToken);
    Task<Wallet> GetOrCreateWalletAsync(string userId, string asset, CancellationToken cancellationToken);
    Task<IReadOnlyList<Wallet>> GetWalletsAsync(string userId, CancellationToken cancellationToken);
    Task<RiskLimit> GetOrCreateRiskLimitAsync(string userId, CancellationToken cancellationToken);
    Task<Order?> FindOrderAsync(Guid orderId, CancellationToken cancellationToken);
    Task UpsertOrderAsync(Order order, CancellationToken cancellationToken);
    Task AddTradesAsync(IEnumerable<Trade> trades, CancellationToken cancellationToken);
    Task AddAuditAsync(AuditLog auditLog, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
