using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Butler.Bot.Core.TargetGroup;

public interface ITargetGroupBot
{
    Task<ChatMemberStatus?> GetMemberStatusAsync(long userId, CancellationToken cancellationToken);

    Task DeclineJoinRequestAsync(long userId, CancellationToken cancellationToken);

    Task ApproveJoinRequestAsync(long userId, CancellationToken cancellationToken);

    Task<Message> SayHelloToNewMemberAsync(User user, string whois, CancellationToken cancellationToken);

    Task SayHelloToUnknownNewMemberAsync(User user, CancellationToken cancellationToken);

    Task SayHelloAgainAsync(User user, CancellationToken cancellationToken);

    Task DeleteUserAsync(long userId, CancellationToken cancellationToken);

    Task DeleteMessageAsync(int messageId, CancellationToken cancellationToken);

    Task SayLeavingToChangeWhoisAsync(User user, CancellationToken cancellationToken);
}