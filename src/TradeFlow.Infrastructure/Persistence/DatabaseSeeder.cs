using Microsoft.EntityFrameworkCore;
using TradeFlow.Domain.Entities;

namespace TradeFlow.Infrastructure.Persistence;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(TradeFlowDbContext db, CancellationToken cancellationToken = default)
    {
        await db.Database.EnsureCreatedAsync(cancellationToken);
        if (!await db.TradingPairs.AnyAsync(cancellationToken))
        {
            db.TradingPairs.AddRange(
                new TradingPair("BTC-USD", "BTC", "USD"),
                new TradingPair("ETH-USD", "ETH", "USD"));
            await db.SaveChangesAsync(cancellationToken);
        }
    }
}
