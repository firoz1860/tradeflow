using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TradeFlow.Domain.Entities;

namespace TradeFlow.Infrastructure.Persistence.Configurations;

public sealed class WalletConfiguration : IEntityTypeConfiguration<Wallet>
{
    public void Configure(EntityTypeBuilder<Wallet> builder)
    {
        builder.ToTable("wallets");
        builder.HasKey(wallet => wallet.Id);
        builder.Property(wallet => wallet.UserId).HasMaxLength(64).IsRequired();
        builder.Property(wallet => wallet.Asset).HasMaxLength(16).IsRequired();
        builder.Property(wallet => wallet.Available).HasPrecision(28, 10);
        builder.Property(wallet => wallet.Reserved).HasPrecision(28, 10);
        builder.HasIndex(wallet => new { wallet.UserId, wallet.Asset }).IsUnique();
    }
}
