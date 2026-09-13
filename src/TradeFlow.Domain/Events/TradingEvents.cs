namespace TradeFlow.Domain.Events;

public sealed record OrderPlacedEvent(Guid OrderId, string Symbol, string UserId, DateTimeOffset OccurredAt);
public sealed record TradeExecutedEvent(Guid TradeId, string Symbol, DateTimeOffset OccurredAt);
public sealed record OrderRejectedEvent(Guid OrderId, string Symbol, string Reason, DateTimeOffset OccurredAt);
