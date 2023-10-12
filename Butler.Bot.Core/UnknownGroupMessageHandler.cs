using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Butler.Bot.Core;

public class UnknownGroupMessageHandler : IUpdateHandler
{
    private readonly ButlerOptions options;
    private readonly ILogger<UnknownGroupMessageHandler> logger;

    public UnknownGroupMessageHandler(IOptions<ButlerOptions> options, ILogger<UnknownGroupMessageHandler> logger)
    {
        this.options = options.Value;
        this.logger = logger;
    }

    public Task<bool> TryHandleUpdateAsync(Update update, CancellationToken cancellationToken)
    {
        if (update.Message == null || update.Message.Chat.Type == ChatType.Private) return Task.FromResult(false);

        var chat = update.Message.Chat;

        if (chat.Id == options.TargetGroupId || chat.Id == options.AdminGroupId) return Task.FromResult(false);

        logger.LogWarning("Unknown public group: {ChatId}, title: {Title}, type: {Type}", chat.Id, chat.Title, chat.Type);

        return Task.FromResult(true);
    }
}

