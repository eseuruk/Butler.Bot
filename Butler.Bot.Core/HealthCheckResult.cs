namespace Butler.Bot.Core;

public enum HealthStatus
{
    Unhealthy,
    Degraded,
    Healthy
}

public class HealthCheckResult
{
    public HealthCheckResult(HealthStatus status, string description)
    {
        Status = status;
        Description = description;
    }

    public HealthStatus Status { get; }

    public string Description { get; }

    public static HealthCheckResult Unhealthy(string description)
    {
        return new HealthCheckResult(HealthStatus.Unhealthy, description);
    }

    public static HealthCheckResult Degraded(string description)
    {
        return new HealthCheckResult(HealthStatus.Degraded, description);
    }

    public static HealthCheckResult Healthy(string description)
    {
        return new HealthCheckResult(HealthStatus.Healthy, description);
    }
}
