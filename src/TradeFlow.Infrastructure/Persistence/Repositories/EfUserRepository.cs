using Microsoft.EntityFrameworkCore;
using TradeFlow.Application.Abstractions;
using TradeFlow.Domain.Entities;
using TradeFlow.Infrastructure.Persistence;

namespace TradeFlow.Infrastructure.Persistence.Repositories;

public sealed class EfUserRepository(TradeFlowDbContext db) : IUserRepository
{
    public Task<User?> FindByEmailAsync(string email, CancellationToken cancellationToken) =>
        db.Users.SingleOrDefaultAsync(user => user.Email == email.Trim().ToLower(), cancellationToken);

    public Task AddAsync(User user, CancellationToken cancellationToken)
    {
        db.Users.Add(user);
        db.Wallets.AddRange(new Wallet(user.Id.ToString(), "USD", 100_000m), new Wallet(user.Id.ToString(), "BTC", 5m), new Wallet(user.Id.ToString(), "ETH", 100m));
        db.RiskLimits.Add(new RiskLimit(user.Id.ToString()));
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken) => db.SaveChangesAsync(cancellationToken);
}
