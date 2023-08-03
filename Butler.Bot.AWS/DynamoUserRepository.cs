using Butler.Bot.Core;

namespace Butler.Bot.AWS;

public class DynamoUserRepository : IUserRepository
{
    private readonly DynamoJoinRequestTable table;
    private readonly ILogger<DynamoUserRepository> logger;

    public DynamoUserRepository(DynamoJoinRequestTable table, ILogger<DynamoUserRepository> logger)
    {
        this.table = table;
        this.logger = logger;
    }

    public async Task<JoinRequest?> FindJoinRequestAsync(long userId, CancellationToken cancellationToken)
    {
        var request = await table.GetItemAsync(userId, cancellationToken);

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
        await table.PutItemAsync(request, cancellationToken);

        logger.LogInformation("Request created: {UserId}, whois: {Whois}, whoisMessageId: {WhoisMessageId}, userChatId: {UserChatId}", request.UserId, request.Whois, request.WhoisMessageId, request.UserChatId);
    }

    public async Task UpdateJoinRequestAsync(JoinRequest request, CancellationToken cancellationToken)
    {
        await table.PutItemAsync(request, cancellationToken);

        logger.LogInformation("Request updated: {UserId}, whois: {Whois}, whoisMessageId: {WhoisMessageId}, userChatId: {UserChatId}", request.UserId, request.Whois, request.WhoisMessageId, request.UserChatId);
    }
}
