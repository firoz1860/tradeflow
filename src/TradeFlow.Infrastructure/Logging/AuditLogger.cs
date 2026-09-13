using Microsoft.Extensions.Logging;

namespace TradeFlow.Infrastructure.Logging;

public sealed class AuditLogger(ILogger<AuditLogger> logger)
{
    public void Record(string actorId, string action, string details) =>
        logger.LogInformation("Audit {ActorId} {Action} {Details}", actorId, action, details);
}
