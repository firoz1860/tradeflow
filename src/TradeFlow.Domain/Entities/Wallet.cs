using TradeFlow.Domain.Common;

namespace TradeFlow.Domain.Entities;

public sealed class Wallet
{
    private Wallet() { }

    public Wallet(string userId, string asset, decimal available = 0m)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        Asset = asset.ToUpperInvariant();
        Available = available;
    }

    public Guid Id { get; private set; }
    public string UserId { get; private set; } = string.Empty;
    public string Asset { get; private set; } = string.Empty;
    public decimal Available { get; private set; }
    public decimal Reserved { get; private set; }

    public void Reserve(decimal amount)
    {
        if (amount < 0 || Available < amount) throw new DomainException($"Insufficient {Asset} balance.");
        Available -= amount;
        Reserved += amount;
    }

    public void Release(decimal amount)
    {
        if (amount < 0 || Reserved < amount) throw new DomainException($"Invalid {Asset} release.");
        Reserved -= amount;
        Available += amount;
    }

    public void ConsumeReserved(decimal amount)
    {
        if (amount < 0 || Reserved < amount) throw new DomainException($"Invalid {Asset} settlement.");
        Reserved -= amount;
    }

    public void Credit(decimal amount)
    {
        if (amount < 0) throw new DomainException("Credit cannot be negative.");
        Available += amount;
    }
}
