using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;

namespace Butler.Bot.Core;

public class BotChatStatusHandler : UpdateHandlerBase
{
    public BotChatStatusHandler(IButlerBot butler, IUserRepository userRepository, ILogger<BotChatStatusHandler> logger)
        : base(butler, userRepository, logger)
    {}

    public override Task<bool> TryHandleUpdateAsync(Update update, CancellationToken cancellationToken)
    {
        if (update.MyChatMember == null) return Task.FromResult(false);

        var chat = update.MyChatMember.Chat;
        var oldStatus = update.MyChatMember.OldChatMember.Status;
        var newStatus = update.MyChatMember.NewChatMember.Status;

        Logger.LogWarning("Bot status changed in chat: {ChatId}, title: {Title}, type: {Type}, oldStatus: {OldStatus}, newStatus: {NreStatus}", chat.Id, chat.Title, chat.Type, oldStatus, newStatus);

        return Task.FromResult(true);
    }
}

