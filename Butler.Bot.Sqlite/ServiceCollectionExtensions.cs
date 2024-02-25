namespace Butler.Bot.Sqlite;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSqliteUserRepository(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<SqliteUserRepositoryOptions>(config);

        services.AddSingleton<SqliteDatabase>();
        services.AddSingleton<IUserRepository, SqliteUserRepository>();

        services.AddSingleton<IComponentHealthCheck, SqliteHealthCheck>();

        return services;
    }
}
