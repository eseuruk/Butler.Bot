namespace Butler.Bot.Core;

public interface IUserRepository
{
    Task<JoinRequest?> FindJoinRequest(long userId);

    Task CreateJoinRequestAsync(JoinRequest request);

    Task UpdateJoinRequestAsync(JoinRequest request);

    public async Task<JoinRequest> FindOrCreateRequest(long userId)
    {
        var request = await FindJoinRequest(userId);

        if (request == null)
        {
            request = new JoinRequest { UserId = userId };
            await CreateJoinRequestAsync(request);
        }

        return request;
    }
}
