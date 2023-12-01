namespace Butler.Bot.Local;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUserRepository(this IServiceCollection services, IConfiguration config)
    {
        var repositoryType = config.GetValue("RepositoryType", RepositoryType.Sqlite);

        switch(repositoryType)
        {
            case RepositoryType.Sqlite:
                return services.AddSqliteUserRepository(config.GetSection("Sqlite"));

            case RepositoryType.DynamoDB:
                return services.AddDynamoUserRepository(config.GetSection("DynamoDB"));

            case RepositoryType.InMemory:
                return services.AddInmemoryUserRepository();
        }

        return services;
    }

    private static IServiceCollection AddInmemoryUserRepository(this IServiceCollection services)
    {
        services.AddSingleton<IUserRepository, InMemoryRequestRepository>();
        return services;
    }
}
