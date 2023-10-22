namespace Butler.Bot.Core.UserChat;

public interface IUserChatBot
{
    Task StopQuerySpinnerAsync(string callbackQueryId, CancellationToken cancellationToken);

    Task SayBotVersionAsync(long userChatId, CancellationToken cancellationToken);

    Task SayHelloAsync(long userChatId, CancellationToken cancellationToken);

    Task SayConfusedAsync(long userChatId, CancellationToken cancellationToken);

    Task AskForWhoisAsync(long userChatId, CancellationToken cancellationToken);

    Task WarnWhoisValidationFailedAsync(long userChatId, string validationError, CancellationToken cancellationToken);

    Task SayWhoisOkAndAskToRequestAccessAsync(long userChatId, CancellationToken cancellationToken);

    Task SayUsedToBeMemberAsync(long userChatId, CancellationToken cancellationToken);

    Task SayAlreadyMemberAsync(long userChatId, CancellationToken cancellationToken);

    Task SayBlockedAsync(long userChatId, CancellationToken cancellationToken);

    Task TrySayingRequestApprovedAsync(long userChatId, CancellationToken cancellationToken);

    Task TrySayingRequestDeclinedAsync(long userChatId, CancellationToken cancellationToken);

    Task TrySayingUserDeletedAsync(long userChatId, CancellationToken cancellationToken);
}

