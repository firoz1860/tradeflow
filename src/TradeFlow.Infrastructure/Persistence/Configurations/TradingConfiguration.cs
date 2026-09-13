using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TradeFlow.Domain.Entities;

namespace TradeFlow.Infrastructure.Persistence.Configurations;

public sealed class TradingPairConfiguration : IEntityTypeConfiguration<TradingPair>
{
    public void Configure(EntityTypeBuilder<TradingPair> builder)
    {
        builder.ToTable("trading_pairs");
        builder.HasKey(pair => pair.Id);
        builder.Property(pair => pair.Symbol).HasMaxLength(20).IsRequired();
        builder.HasIndex(pair => pair.Symbol).IsUnique();
        builder.Property(pair => pair.BaseAsset).HasMaxLength(16).IsRequired();
        builder.Property(pair => pair.QuoteAsset).HasMaxLength(16).IsRequired();
    }
}

public sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("orders");
        builder.HasKey(order => order.Id);
        builder.Property(order => order.UserId).HasMaxLength(64).IsRequired();
        builder.Property(order => order.Symbol).HasMaxLength(20).IsRequired();
        builder.Property(order => order.Side).HasConversion<string>().HasMaxLength(8);
        builder.Property(order => order.Type).HasConversion<string>().HasMaxLength(8);
        builder.Property(order => order.TimeInForce).HasConversion<string>().HasMaxLength(32);
        builder.Property(order => order.Status).HasConversion<string>().HasMaxLength(32);
        builder.Property(order => order.Price).HasPrecision(28, 10);
        builder.Property(order => order.ReferencePrice).HasPrecision(28, 10);
        builder.Property(order => order.OriginalQuantity).HasPrecision(28, 10);
        builder.Property(order => order.RemainingQuantity).HasPrecision(28, 10);
        builder.HasIndex(order => new { order.UserId, order.Status });
        builder.HasIndex(order => new { order.Symbol, order.CreatedAt });
    }
}

public sealed class TradeConfiguration : IEntityTypeConfiguration<Trade>
{
    public void Configure(EntityTypeBuilder<Trade> builder)
    {
        builder.ToTable("trades");
        builder.HasKey(trade => trade.Id);
        builder.Property(trade => trade.Symbol).HasMaxLength(20).IsRequired();
        builder.Property(trade => trade.MakerUserId).HasMaxLength(64).IsRequired();
        builder.Property(trade => trade.TakerUserId).HasMaxLength(64).IsRequired();
        builder.Property(trade => trade.MakerSide).HasConversion<string>().HasMaxLength(8);
        builder.Property(trade => trade.Price).HasPrecision(28, 10);
        builder.Property(trade => trade.Quantity).HasPrecision(28, 10);
        builder.HasIndex(trade => new { trade.Symbol, trade.ExecutedAt });
    }
}

public sealed class RiskLimitConfiguration : IEntityTypeConfiguration<RiskLimit>
{
    public void Configure(EntityTypeBuilder<RiskLimit> builder)
    {
        builder.ToTable("risk_limits");
        builder.HasKey(limit => limit.Id);
        builder.Property(limit => limit.UserId).HasMaxLength(64).IsRequired();
        builder.Property(limit => limit.MaxOrderQuantity).HasPrecision(28, 10);
        builder.Property(limit => limit.MaxOrderNotional).HasPrecision(28, 10);
        builder.HasIndex(limit => limit.UserId).IsUnique();
    }
}

public sealed class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("audit_logs");
        builder.HasKey(log => log.Id);
        builder.Property(log => log.ActorId).HasMaxLength(64).IsRequired();
        builder.Property(log => log.Action).HasMaxLength(64).IsRequired();
        builder.Property(log => log.Details).HasMaxLength(2_000).IsRequired();
        builder.HasIndex(log => log.CreatedAt);
    }
}
