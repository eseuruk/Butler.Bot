using Amazon.DynamoDBv2.DataModel;
using Butler.Bot.Core;

namespace Butler.Bot.AWS;

public class DynamoRequestRepository : IUserRepository
{
    private readonly IDynamoDBContext context;
    private readonly ILogger<DynamoRequestRepository> logger;

    public DynamoRequestRepository(IDynamoDBContext context, ILogger<DynamoRequestRepository> logger)
    {
        this.context = context;
        this.logger = logger;
    }

    public async Task<JoinRequest?> FindJoinRequest(long userId)
    {
        var requestDTO = await context.LoadAsync<DynamoJoinRequest>(userId);

        if (requestDTO == null)
        {
            logger.LogInformation("Request not found: {UserId}", userId);
            return null;
        }

        var request = new JoinRequest
        {
            UserId = requestDTO.UserId,
            Whois = requestDTO.Whois,
            WhoisMessageId = requestDTO.WhoisMessageId,
            UserChatId = requestDTO.UserChatId
        };

        logger.LogInformation("Request found: {UserId}, whois: {Whois}, whoisMessageId: {WhoisMessageId}, userChatId: {UserChatId}", request.UserId, request.Whois, request.WhoisMessageId, request.UserChatId);
        return request;
    }

    public async Task CreateJoinRequestAsync(JoinRequest request)
    {
        await PutJoinRequestAsync(request);

        logger.LogInformation("Request created: {UserId}, whois: {Whois}, whoisMessageId: {WhoisMessageId}, userChatId: {UserChatId}", request.UserId, request.Whois, request.WhoisMessageId, request.UserChatId);
    }

    public async Task UpdateJoinRequestAsync(JoinRequest request)
    {
        await PutJoinRequestAsync(request);

        logger.LogInformation("Request updated: {UserId}, whois: {Whois}, whoisMessageId: {WhoisMessageId}, userChatId: {UserChatId}", request.UserId, request.Whois, request.WhoisMessageId, request.UserChatId);
    }

    private async Task PutJoinRequestAsync(JoinRequest request)
    {
        var requestDTO = new DynamoJoinRequest
        {
            UserId = request.UserId,
            Whois = request.Whois,
            WhoisMessageId = request.WhoisMessageId,
            UserChatId = request.UserChatId
        };

        await context.SaveAsync(requestDTO);
    }
}
