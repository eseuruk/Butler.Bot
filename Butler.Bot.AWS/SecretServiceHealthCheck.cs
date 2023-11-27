namespace Butler.Bot.AWS;

public class SecretServiceHealthCheck : IHealthCheck
{
    private readonly TelegramApiOptions options;

    public SecretServiceHealthCheck(IOptions<TelegramApiOptions> options)
    {
        this.options = options.Value;
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        if (!options.SecretTokenValidation)
        {
            return Task.FromResult(HealthCheckResult.Degraded("Secret validation is off"));
        }
        else
        {
            return Task.FromResult(HealthCheckResult.Healthy("Secret validation is on"));
        }
    }
}