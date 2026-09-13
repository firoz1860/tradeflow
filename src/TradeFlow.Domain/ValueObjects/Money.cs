using TradeFlow.Domain.Common;

namespace TradeFlow.Domain.ValueObjects;

public readonly record struct Money
{
    public Money(decimal amount, string currency)
    {
        if (amount < 0) throw new DomainException("Money amount cannot be negative.");
        if (string.IsNullOrWhiteSpace(currency)) throw new DomainException("Currency is required.");
        Amount = amount;
        Currency = currency.ToUpperInvariant();
    }

    public decimal Amount { get; }
    public string Currency { get; }
}
