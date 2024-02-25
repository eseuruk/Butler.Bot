namespace Butler.Bot.Core;

public interface IHealthCheckService
{
    Task<HealthCheckReport> CheckHealthAsync(BotExecutionContext context, ComponentFilter filter, CancellationToken cancellationToken);
}
