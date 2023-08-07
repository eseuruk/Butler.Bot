using Butler.Bot.Core;
using Microsoft.Extensions.Logging;

namespace Butler.Bot.SQLite;

public class SQLiteUserRepository : IUserRepository
{
    private readonly JoinRequestTable table;
    private readonly ILogger<SQLiteUserRepository> logger;
    
    public SQLiteUserRepository(JoinRequestTable table, ILogger<SQLiteUserRepository> logger)
    {
        this.table = table;
        this.logger = logger;
    }

    public async Task<JoinRequest?> FindJoinRequestAsync(long userId, CancellationToken cancellationToken)
    {
        var request = await table.SelectRecordAsync(userId, cancellationToken);

        if (request == null)
        {
            logger.LogInformation("Request not found: {UserId}", userId);
            return null;
        }

        logger.LogInformation("Request found: {UserId}, whois: {Whois}, whoisMessageId: {WhoisMessageId}, userChatId: {UserChatId}", request.UserId, request.Whois, request.WhoisMessageId, request.UserChatId);
        return request;
    }

    public async Task CreateJoinRequestAsync(JoinRequest request, CancellationToken cancellationToken)
    {
        await table.CreateRecordAsync(request, cancellationToken);

        logger.LogInformation("Request created: {UserId}, whois: {Whois}, whoisMessageId: {WhoisMessageId}, userChatId: {UserChatId}", request.UserId, request.Whois, request.WhoisMessageId, request.UserChatId);
    }

    public async Task UpdateJoinRequestAsync(JoinRequest request, CancellationToken cancellationToken)
    {
        await table.UpdateRecordAsync(request, cancellationToken);

        logger.LogInformation("Request updated: {UserId}, whois: {Whois}, whoisMessageId: {WhoisMessageId}, userChatId: {UserChatId}", request.UserId, request.Whois, request.WhoisMessageId, request.UserChatId);
    }
}
