namespace Butler.Bot.Core;

public class HealthCheckService : IHealthCheckService
{
    public readonly IReadOnlyCollection<IComponentHealthCheck> componentHealthChecks;
    public readonly ILogger<InstallService> logger;
    
    public HealthCheckService(IEnumerable<IComponentHealthCheck> componentHealthChecks, ILogger<InstallService> logger)
    {
        this.componentHealthChecks = componentHealthChecks.ToList();
        this.logger = logger;
    }

    public async Task<HealthCheckReport> CheckHealthAsync(BotExecutionContext context, ComponentFilter filter, CancellationToken cancellationToken)
    {
        var results = new Dictionary<string, HealthCheckResult>();

        foreach (var healthCheck in componentHealthChecks)
        {
            if (filter.AcceptComponent(healthCheck.ComponentId))
            {
                logger.LogInformation("Checking health of component: {componentId}", healthCheck.ComponentId);

                var result = await healthCheck.CheckHealthAsync(context, cancellationToken);
                results.Add(healthCheck.ComponentId, result);
            }
        }

        return new HealthCheckReport(results);
    }
}
