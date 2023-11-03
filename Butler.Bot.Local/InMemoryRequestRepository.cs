using Butler.Bot.Core;

namespace Butler.Bot.Local;

public class InMemoryRequestRepository : IUserRepository
{
    private readonly Dictionary<long, JoinRequest> contaner = new Dictionary<long, JoinRequest>();
    private readonly ILogger<InMemoryRequestRepository> logger;

    public InMemoryRequestRepository(ILogger<InMemoryRequestRepository> logger)
    {
        this.logger = logger;
    }

    public Task<JoinRequest?> FindJoinRequestAsync(long userId, CancellationToken cancellationToken)
    {
        JoinRequest? request;
        if (contaner.TryGetValue(userId, out request))
        {
            logger.LogInformation("Request is found: {UserId}, whois: {Whois}, whoisMessageId: {WhoisMessageId}, userChatId: {UserChatId}", request.UserId, request.Whois, request.WhoisMessageId, request.UserChatId);
        }
        else
        {
            logger.LogInformation("Request is not found: {UserId}", userId);
        }

        return Task.FromResult(request);
    }

    public Task CreateJoinRequestAsync(JoinRequest request, CancellationToken cancellationToken)
    {
        contaner.Add(request.UserId, request);

        logger.LogInformation("Request is created: {UserId}, whois: {Whois}, whoisMessageId: {WhoisMessageId}, userChatId: {UserChatId}", request.UserId, request.Whois, request.WhoisMessageId, request.UserChatId);
        return Task.CompletedTask;
    }

    public Task UpdateJoinRequestAsync(JoinRequest request, CancellationToken cancellationToken)
    {
        contaner[request.UserId] = request;

        logger.LogInformation("Request is updated: {UserId}, whois: {Whois}, whoisMessageId: {WhoisMessageId}, userChatId: {UserChatId}", request.UserId, request.Whois, request.WhoisMessageId, request.UserChatId);
        return Task.CompletedTask;
    }

    public Task DeleteJoinRequestAsync(long userId, CancellationToken cancellationToken)
    {
        bool removed = contaner.Remove(userId);

        if (removed)
        {
            logger.LogInformation("Request is deleted: {UserId}", userId);
        }
        else
        {
            logger.LogInformation("Request is not found so can not be deleted: {UserId}", userId);
        }

        return Task.CompletedTask;
    }
}

