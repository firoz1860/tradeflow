using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TradeFlow.Contracts.Requests;
using TradeFlow.Infrastructure.Persistence;

namespace TradeFlow.Api.Controllers;

[ApiController]
[Authorize(Roles = "RiskAdmin,SystemAdmin")]
[Route("api/admin")]
public sealed class AdminController(TradeFlowDbContext db) : ControllerBase
{
    [HttpPut("pairs/{symbol}/status")]
    public async Task<IActionResult> SetPairStatus(string symbol, UpdateTradingPairRequest request, CancellationToken cancellationToken)
    {
        var pair = await db.TradingPairs.SingleOrDefaultAsync(item => item.Symbol == symbol.ToUpper(), cancellationToken);
        if (pair is null) return NotFound(new { error = "Trading pair not found." });
        pair.SetHalted(request.IsHalted);
        await db.SaveChangesAsync(cancellationToken);
        return Ok(new { pair.Symbol, pair.IsHalted });
    }
}
