namespace Butler.Bot.Core;

public interface IComponentHealthCheck
{
    string ComponentId { get; }

    Task<HealthCheckResult> CheckHealthAsync(BotExecutionContext context, CancellationToken cancellationToken);
}
