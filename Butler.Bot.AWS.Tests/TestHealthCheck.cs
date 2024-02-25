namespace Butler.Bot.AWS.Tests;

public class TestHealthCheck : IComponentHealthCheck
{
    public TestHealthCheck(string componetId)
    {
        ComponentId = componetId;
    }

    public string ComponentId { get; }

    public Task<HealthCheckResult> CheckHealthAsync(BotExecutionContext context, CancellationToken cancellationToken)
    {
        return Task.FromResult(HealthCheckResult.Healthy($"{ComponentId} ok"));
    }
}
