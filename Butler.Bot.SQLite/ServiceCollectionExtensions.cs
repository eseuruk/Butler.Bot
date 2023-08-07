using Butler.Bot.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Butler.Bot.SQLite;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSQLiteUserRepository(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<SQLiteUserRepositoryOptions>(config);

        services.AddSingleton<JoinRequestTable>();
        services.AddSingleton<IUserRepository, SQLiteUserRepository>();

        return services;
    }
}
