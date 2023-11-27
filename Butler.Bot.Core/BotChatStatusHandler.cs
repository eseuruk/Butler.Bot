namespace Butler.Bot.Core;

public class BotChatStatusHandler : IUpdateHandler
{
    private readonly ILogger<BotChatStatusHandler> logger;

    public BotChatStatusHandler(ILogger<BotChatStatusHandler> logger)
    {
        this.logger = logger;
    }

    public Task<bool> TryHandleUpdateAsync(Update update, CancellationToken cancellationToken)
    {
        if (update.MyChatMember == null) return Task.FromResult(false);

        var chat = update.MyChatMember.Chat;
        var oldStatus = update.MyChatMember.OldChatMember.Status;
        var newStatus = update.MyChatMember.NewChatMember.Status;

        logger.LogWarning("Bot status changed in chat: {ChatId}, title: {Title}, type: {Type}, oldStatus: {OldStatus}, newStatus: {NreStatus}", chat.Id, chat.Title, chat.Type, oldStatus, newStatus);

        return Task.FromResult(true);
    }
}

