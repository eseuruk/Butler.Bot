namespace Butler.Bot.AWS;

public static class HealthChecksExtensions
{
    public static IHealthChecksBuilder AddSecretServiceCheck(this IHealthChecksBuilder builder)
    {
        return builder.AddCheck<SecretServiceHealthCheck>("SecretService");
    }
}
