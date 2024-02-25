namespace Butler.Bot.DynamoDB;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDynamoUserRepository(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<DynamoUserRepositoryOptions>(config);

        services.AddSingleton<IAmazonDynamoDB>(DynamoDBClientFactory.CreateClient);
        services.AddSingleton<DynamoJoinRequestTable>();
        services.AddSingleton<IUserRepository, DynamoUserRepository>();

        services.AddSingleton<IComponentHealthCheck, DynamoHealthCheck>();

        return services;
    }
}
