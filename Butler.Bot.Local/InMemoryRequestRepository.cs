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
            logger.LogInformation("Request found: {UserId}, whois: {Whois}, whoisMessageId: {WhoisMessageId}, userChatId: {UserChatId}", request.UserId, request.Whois, request.WhoisMessageId, request.UserChatId);
        }
        else
        {
            logger.LogInformation("Request not found: {UserId}", userId);
        }

        return Task.FromResult(request);
    }

    public Task CreateJoinRequestAsync(JoinRequest request, CancellationToken cancellationToken)
    {
        contaner.Add(request.UserId, request);

        logger.LogInformation("Request created: {UserId}, whois: {Whois}, whoisMessageId: {WhoisMessageId}, userChatId: {UserChatId}", request.UserId, request.Whois, request.WhoisMessageId, request.UserChatId);
        return Task.CompletedTask;
    }

    public Task UpdateJoinRequestAsync(JoinRequest request, CancellationToken cancellationToken)
    {
        contaner[request.UserId] = request;

        logger.LogInformation("Request updated: {UserId}, whois: {Whois}, whoisMessageId: {WhoisMessageId}, userChatId: {UserChatId}", request.UserId, request.Whois, request.WhoisMessageId, request.UserChatId);
        return Task.CompletedTask;
    }
}

