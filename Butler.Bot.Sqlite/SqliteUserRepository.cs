using Butler.Bot.Core;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Requests.Abstractions;
using Telegram.Bot.Types;

namespace Butler.Bot.Sqlite;

public class SqliteUserRepository : IUserRepository
{
    private readonly SqlightDatabase database;
    private readonly ILogger<SqliteUserRepository> logger;
    
    public SqliteUserRepository(SqlightDatabase database, ILogger<SqliteUserRepository> logger)
    {
        this.database = database;
        this.logger = logger;
    }

    public Task<JoinRequest?> FindJoinRequestAsync(long userId, CancellationToken cancellationToken)
    {
        if( !database.IsExist() )
        {
            logger.LogInformation("Database does not exist, so no request found: {UserId}, databaseFileName: {DatabaseFileName}", userId, database.FileName);
            return Task.FromResult<JoinRequest?>(null);
        }

        var request = database.SelectUserRequest(userId);
        if (request == null)
        {
            logger.LogInformation("Request not found: {UserId}", userId);
            return Task.FromResult<JoinRequest?>(null);
        }

        logger.LogInformation("Request is found: {UserId}, whois: {Whois}, whoisMessageId: {WhoisMessageId}, userChatId: {UserChatId}", request.UserId, request.Whois, request.WhoisMessageId, request.UserChatId);
        return Task.FromResult<JoinRequest?>(request);
    }

    public Task CreateJoinRequestAsync(JoinRequest request, CancellationToken cancellationToken)
    {
        CreateDatabaseIfNotExist();

        database.InsertUserRequest(request);

        logger.LogInformation("Request is created: {UserId}, whois: {Whois}, whoisMessageId: {WhoisMessageId}, userChatId: {UserChatId}", request.UserId, request.Whois, request.WhoisMessageId, request.UserChatId);
        return Task.CompletedTask;
    }

    public Task UpdateJoinRequestAsync(JoinRequest request, CancellationToken cancellationToken)
    {
        CreateDatabaseIfNotExist();

        database.UpdateUserRequest(request);
        
        logger.LogInformation("Request is updated: {UserId}, whois: {Whois}, whoisMessageId: {WhoisMessageId}, userChatId: {UserChatId}", request.UserId, request.Whois, request.WhoisMessageId, request.UserChatId);
        return Task.CompletedTask;
    }

    public Task DeleteJoinRequestAsync(long userId, CancellationToken cancellationToken)
    {
        if (!database.IsExist())
        {
            logger.LogInformation("Database does not exist, so nothing to delete for: {UserId}, databaseFileName: {DatabaseFileName}", userId, database.FileName);
            return Task.FromResult<JoinRequest?>(null);
        }

        database.DeleteUserRequest(userId);

        logger.LogInformation("Request is deleted: {UserId}", userId);
        return Task.CompletedTask;
    }
    private void CreateDatabaseIfNotExist()
    {
        if (database.IsExist()) return;

        database.Create();
        logger.LogInformation("New database created: {DatabaseFileName}", database.FileName);
    }
}
