using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Butler.Bot.Core;

public class UnknownGroupMessageHandler : UpdateHandlerBase
{
    public UnknownGroupMessageHandler(ButlerBot butler, IUserRepository userRepository, ILogger<UnknownGroupMessageHandler> logger)
        : base(butler, userRepository, logger)
    {}

    public override Task<bool> TryHandleUpdateAsync(Update update, CancellationToken cancellationToken)
    {
        if (update.Message == null || update.Message.Chat.Type == ChatType.Private) return Task.FromResult(false);

        var chat = update.Message.Chat;

        if (chat.Id == Butler.Options.TargetGroupId || chat.Id == Butler.Options.AdminGroupId) return Task.FromResult(false);

        Logger.LogWarning("Unknown public group: {ChatId}, title: {Title}, type: {Type}", chat.Id, chat.Title, chat.Type);

        return Task.FromResult(true);
    }
}

