using Telegram.Bot.Types;

namespace Butler.Bot.Core.AdminGroup;

public interface IAdminGroupBot
{
    Task StopQuerySpinnerAsync(string callbackQueryId, CancellationToken cancellationToken);

    Task MarkJoinRequestAsApprovedAsync(int messageId, User admin, CancellationToken cancellationToken);

    Task MarkJoinRequestAsDeclinedAsync(int messageId, User admin, CancellationToken cancellationToken);

    Task MarkUserAsDeletedAsync(int messageId, User admin, CancellationToken cancellationToken);

    Task<Message> ReportJoinRequestAsync(User user, string whois, CancellationToken cancellationToken);

    Task ReportUserAddedAsync(User user, string whois, CancellationToken cancellationToken);
}