using Microsoft.EntityFrameworkCore;
using TradeFlow.Application.Abstractions;
using TradeFlow.Domain.Entities;
using TradeFlow.Infrastructure.Persistence;

namespace TradeFlow.Infrastructure.Persistence.Repositories;

public sealed class EfTradingRepository(TradeFlowDbContext db) : ITradingRepository
{
    public Task<TradingPair?> FindPairAsync(string symbol, CancellationToken cancellationToken) =>
        db.TradingPairs.SingleOrDefaultAsync(pair => pair.Symbol == symbol.ToUpper(), cancellationToken);

    public async Task<Wallet> GetOrCreateWalletAsync(string userId, string asset, CancellationToken cancellationToken)
    {
        var wallet = await db.Wallets.SingleOrDefaultAsync(item => item.UserId == userId && item.Asset == asset.ToUpper(), cancellationToken);
        if (wallet is not null) return wallet;
        wallet = new Wallet(userId, asset);
        db.Wallets.Add(wallet);
        return wallet;
    }

    public async Task<IReadOnlyList<Wallet>> GetWalletsAsync(string userId, CancellationToken cancellationToken) =>
        await db.Wallets.Where(wallet => wallet.UserId == userId).OrderBy(wallet => wallet.Asset).ToListAsync(cancellationToken);

    public async Task<RiskLimit> GetOrCreateRiskLimitAsync(string userId, CancellationToken cancellationToken)
    {
        var limit = await db.RiskLimits.SingleOrDefaultAsync(item => item.UserId == userId, cancellationToken);
        if (limit is not null) return limit;
        limit = new RiskLimit(userId);
        db.RiskLimits.Add(limit);
        return limit;
    }

    public Task<Order?> FindOrderAsync(Guid orderId, CancellationToken cancellationToken) => db.Orders.SingleOrDefaultAsync(order => order.Id == orderId, cancellationToken);

    public async Task UpsertOrderAsync(Order order, CancellationToken cancellationToken)
    {
        if (db.ChangeTracker.Entries<Order>().Any(entry => entry.Entity.Id == order.Id)) return;
        if (await db.Orders.AnyAsync(item => item.Id == order.Id, cancellationToken)) db.Orders.Update(order);
        else db.Orders.Add(order);
    }

    public async Task AddTradesAsync(IEnumerable<Trade> trades, CancellationToken cancellationToken)
    {
        foreach (var trade in trades)
        {
            if (!await db.Trades.AnyAsync(item => item.Id == trade.Id, cancellationToken)) db.Trades.Add(trade);
        }
    }

    public Task AddAuditAsync(AuditLog auditLog, CancellationToken cancellationToken)
    {
        db.AuditLogs.Add(auditLog);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken) => db.SaveChangesAsync(cancellationToken);
}
