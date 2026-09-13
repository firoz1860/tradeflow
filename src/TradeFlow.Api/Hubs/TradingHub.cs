using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace TradeFlow.Api.Hubs;

[Authorize]
public sealed class TradingHub : Hub
{
    public Task JoinSymbol(string symbol) => Groups.AddToGroupAsync(Context.ConnectionId, $"symbol:{symbol.ToUpperInvariant()}");
    public Task LeaveSymbol(string symbol) => Groups.RemoveFromGroupAsync(Context.ConnectionId, $"symbol:{symbol.ToUpperInvariant()}");
}
