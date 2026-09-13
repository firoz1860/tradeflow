using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TradeFlow.Application.DTOs;
using TradeFlow.Application.Services;
using TradeFlow.Contracts.Requests;
using TradeFlow.Contracts.Responses;
using TradeFlow.Domain.Enums;

namespace TradeFlow.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/orders")]
public sealed class OrdersController(TradingService tradingService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<OrderResponse>> Place(PlaceOrderRequest request, CancellationToken cancellationToken)
    {
        if (request.Type == OrderType.Limit && (!request.Price.HasValue || request.Price <= 0)) return BadRequest(new { error = "Limit orders require a positive price." });
        if (request.Quantity <= 0) return BadRequest(new { error = "Quantity must be greater than zero." });
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await tradingService.PlaceOrderAsync(new PlaceOrderCommand(userId, request.Symbol, request.Side, request.Type, request.Quantity, request.Price, request.ReferencePrice, request.TimeInForce), cancellationToken);
        return result.IsAccepted
            ? Accepted(new { orderId = result.OrderId, tradeIds = result.TradeIds })
            : BadRequest(new { orderId = result.OrderId, error = result.RejectionReason });
    }

    [HttpDelete("{orderId:guid}")]
    public async Task<IActionResult> Cancel(Guid orderId, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await tradingService.CancelOrderAsync(userId, orderId, cancellationToken);
        return result.Success ? NoContent() : BadRequest(new { error = result.Error });
    }
}
