namespace TradeFlow.Domain.Entities;

public sealed class AuditLog
{
    private AuditLog() { }

    public AuditLog(string actorId, string action, string details)
    {
        Id = Guid.NewGuid();
        ActorId = actorId;
        Action = action;
        Details = details;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public Guid Id { get; private set; }
    public string ActorId { get; private set; } = string.Empty;
    public string Action { get; private set; } = string.Empty;
    public string Details { get; private set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; private set; }
}
