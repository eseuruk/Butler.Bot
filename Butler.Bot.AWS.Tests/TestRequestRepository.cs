namespace Butler.Bot.AWS.Tests;

public class TestRequestRepository : IUserRepository
{
    public Task<JoinRequest?> FindJoinRequestAsync(long userId, CancellationToken cancellationToken)
    {
        return Task.FromResult<JoinRequest?>(null);
    }

    public Task CreateJoinRequestAsync(JoinRequest request, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task UpdateJoinRequestAsync(JoinRequest request, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task DeleteJoinRequestAsync(long userId, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}

