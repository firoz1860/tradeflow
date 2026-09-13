using Microsoft.EntityFrameworkCore;
using TradeFlow.Domain.Entities;

namespace TradeFlow.Infrastructure.Persistence;

public sealed class TradeFlowDbContext(DbContextOptions<TradeFlowDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Wallet> Wallets => Set<Wallet>();
    public DbSet<TradingPair> TradingPairs => Set<TradingPair>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<Trade> Trades => Set<Trade>();
    public DbSet<RiskLimit> RiskLimits => Set<RiskLimit>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TradeFlowDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
