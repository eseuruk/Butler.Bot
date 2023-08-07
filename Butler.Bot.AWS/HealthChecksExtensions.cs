namespace Butler.Bot.AWS;

public static class HealthChecksExtensions
{
    public static IHealthChecksBuilder AddDynamoHealthCheck(this IHealthChecksBuilder builder)
    {
        return builder.AddCheck<DynamoHealthCheck>("DynamoUserRepository");
    }

    public static IHealthChecksBuilder AddSecretServiceCheck(this IHealthChecksBuilder builder)
    {
        return builder.AddCheck<SecretServiceHealthCheck>("SecretService");
    }
}
