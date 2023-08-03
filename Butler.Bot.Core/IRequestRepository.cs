namespace Butler.Bot.Core;

public interface IUserRepository
{
    Task<JoinRequest?> FindJoinRequestAsync(long userId, CancellationToken cancellationToken);

    Task CreateJoinRequestAsync(JoinRequest request, CancellationToken cancellationToken);

    Task UpdateJoinRequestAsync(JoinRequest request, CancellationToken cancellationToken);

    public async Task<JoinRequest> FindOrCreateRequestAsync(long userId, CancellationToken cancellationToken)
    {
        var request = await FindJoinRequestAsync(userId, cancellationToken);

        if (request == null)
        {
            request = new JoinRequest { UserId = userId };
            await CreateJoinRequestAsync(request, cancellationToken);
        }

        return request;
    }
}
