namespace TradeFlow.Domain.Entities;

public sealed class TradingPair
{
    private TradingPair() { }

    public TradingPair(string symbol, string baseAsset, string quoteAsset)
    {
        Id = Guid.NewGuid();
        Symbol = symbol.ToUpperInvariant();
        BaseAsset = baseAsset.ToUpperInvariant();
        QuoteAsset = quoteAsset.ToUpperInvariant();
    }

    public Guid Id { get; private set; }
    public string Symbol { get; private set; } = string.Empty;
    public string BaseAsset { get; private set; } = string.Empty;
    public string QuoteAsset { get; private set; } = string.Empty;
    public bool IsHalted { get; private set; }

    public void SetHalted(bool isHalted) => IsHalted = isHalted;
}
