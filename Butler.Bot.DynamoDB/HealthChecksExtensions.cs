namespace Butler.Bot.DynamoDB;

public static class HealthChecksExtensions
{
    public static IHealthChecksBuilder AddDynamoHealthCheck(this IHealthChecksBuilder builder)
    {
        return builder.AddCheck<DynamoHealthCheck>("DynamoUserRepository");
    }
}
