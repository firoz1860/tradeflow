using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TradeFlow.Contracts.Responses;
using TradeFlow.MatchingEngine.Services;
using TradeFlow.Infrastructure.Persistence;

namespace TradeFlow.Api.Controllers;

[ApiController]
[Route("api/market")]
public sealed class MarketDataController(OrderBookRegistry books, TradeFlowDbContext db) : ControllerBase
{
    [HttpGet("pairs")]
    public async Task<IActionResult> Pairs(CancellationToken cancellationToken) =>
        Ok(await db.TradingPairs.OrderBy(pair => pair.Symbol).Select(pair => new { pair.Symbol, pair.BaseAsset, pair.QuoteAsset, pair.IsHalted }).ToListAsync(cancellationToken));

    [HttpGet("order-book/{symbol}")]
    public IActionResult OrderBook(string symbol)
    {
        var snapshot = books.TryGet(symbol, out var queue) && queue is not null
            ? queue.Engine.GetSnapshot()
            : new TradeFlow.MatchingEngine.Services.MatchingEngineService(symbol).GetSnapshot();
        return Ok(snapshot);
    }

    [HttpGet("trades/{symbol}")]
    public async Task<IActionResult> Trades(string symbol, CancellationToken cancellationToken) =>
        Ok((await db.Trades.Where(trade => trade.Symbol == symbol.ToUpper()).OrderByDescending(trade => trade.ExecutedAt).Take(30).ToListAsync(cancellationToken)).Select(TradeResponse.From));
}
