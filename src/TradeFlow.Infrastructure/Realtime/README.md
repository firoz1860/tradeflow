# Real-time adapter boundary

The `ITradingNotifier` interface is defined in Application. Its SignalR implementation is placed in `TradeFlow.Api/Hubs/SignalRTradingNotifier.cs` because it depends on the web-layer `TradingHub`; this prevents Infrastructure from depending on API and creating a circular project reference.
