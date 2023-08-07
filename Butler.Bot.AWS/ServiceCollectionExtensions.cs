using Amazon.DynamoDBv2;
using Butler.Bot.Core;

namespace Butler.Bot.AWS;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDynamoUserRepository(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<DynamoUserRepositoryOptions>(config);

        services.AddAWSService<IAmazonDynamoDB>();
        services.AddSingleton<DynamoJoinRequestTable>();
        services.AddSingleton<IUserRepository, DynamoUserRepository>();

        return services;
    }
}
