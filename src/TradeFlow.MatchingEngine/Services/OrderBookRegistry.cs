using System.Collections.Concurrent;
using TradeFlow.MatchingEngine.Concurrency;

namespace TradeFlow.MatchingEngine.Services;

public sealed class OrderBookRegistry : IAsyncDisposable
{
    private readonly ConcurrentDictionary<string, SymbolOrderQueue> _books = new(StringComparer.OrdinalIgnoreCase);

    public SymbolOrderQueue GetOrCreate(string symbol) => _books.GetOrAdd(symbol.ToUpperInvariant(), static pair => new SymbolOrderQueue(pair));

    public bool TryGet(string symbol, out SymbolOrderQueue? book) => _books.TryGetValue(symbol, out book);

    public async ValueTask DisposeAsync()
    {
        foreach (var book in _books.Values) await book.DisposeAsync();
    }
}
