using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TradeFlow.Application.Abstractions;
using TradeFlow.Contracts.Responses;

namespace TradeFlow.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/portfolio")]
public sealed class PortfolioController(ITradingRepository repository) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var wallets = await repository.GetWalletsAsync(userId, cancellationToken);
        return Ok(wallets.Select(WalletResponse.From));
    }
}
