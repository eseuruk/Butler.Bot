using Telegram.Bot.Types;

namespace Butler.Bot.Core.AdminGroup;

public interface IAdminGroupBot
{
    Task StopQuerySpinnerAsync(string callbackQueryId, CancellationToken cancellationToken);

    Task<Message> ReportJoinRequestAsync(User user, string whois, CancellationToken cancellationToken);

    Task MarkJoinRequestAsApprovedAsync(int messageId, User admin, CancellationToken cancellationToken);

    Task MarkJoinRequestAsDeclinedAsync(int messageId, User admin, CancellationToken cancellationToken);

    Task ReportUserAddedAsync(User user, int whoisMessageId, string whois, CancellationToken cancellationToken);

    Task AskForUserDeleteConfirmationAsync(int messageId, CancellationToken cancellationToken);

    Task CancelUserDeletionAsync(int messageId, CancellationToken cancellationToken);

    Task MarkUserAsDeletedAsync(int messageId, User admin, CancellationToken cancellationToken);
}