namespace Butler.Bot.DynamoDB;

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
            logger.LogInformation("Request is not found: {UserId}", userId);
            return null;
        }

        logger.LogInformation("Request is found: {UserId}, whois: {Whois}, whoisMessageId: {WhoisMessageId}, userChatId: {UserChatId}", request.UserId, request.Whois, request.WhoisMessageId, request.UserChatId);
        return request;
    }

    public async Task CreateJoinRequestAsync(JoinRequest request, CancellationToken cancellationToken)
    {
        await table.PutItemAsync(request, cancellationToken);

        logger.LogInformation("Request is created: {UserId}, whois: {Whois}, whoisMessageId: {WhoisMessageId}, userChatId: {UserChatId}", request.UserId, request.Whois, request.WhoisMessageId, request.UserChatId);
    }

    public async Task UpdateJoinRequestAsync(JoinRequest request, CancellationToken cancellationToken)
    {
        await table.PutItemAsync(request, cancellationToken);

        logger.LogInformation("Request is updated: {UserId}, whois: {Whois}, whoisMessageId: {WhoisMessageId}, userChatId: {UserChatId}", request.UserId, request.Whois, request.WhoisMessageId, request.UserChatId);
    }

    public async Task DeleteJoinRequestAsync(long userId, CancellationToken cancellationToken)
    {
        await table.DeleteItemAsync(userId, cancellationToken);

        logger.LogInformation("Request is deleted: {UserId}", userId);
    }
}
