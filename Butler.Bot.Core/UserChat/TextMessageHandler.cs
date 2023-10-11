using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Butler.Bot.Core.UserChat;

public class TextMessageHandler : UpdateHandlerBase
{
    private readonly IWhoisValidator whoisValidator;

    public TextMessageHandler(IButlerBot butler, IUserRepository userRepository, IWhoisValidator whoisValidator, ILogger<TextMessageHandler> logger)
        : base(butler, userRepository, logger)
    {
        this.whoisValidator = whoisValidator;
    }

    public override async Task<bool> TryHandleUpdateAsync(Update update, CancellationToken cancellationToken)
    {
        // Only handle text chat messages
        if (update.Message == null || update.Message.Text == null) return false;

        // Only handle messages from private chat
        if (update.Message.Chat.Type != ChatType.Private) return false;

        // Only handle messages from real users
        if (update.Message.From == null || update.Message.From.IsBot) return false;

        await DoHandleTextMessage(update.Message.Chat, update.Message.From, update.Message.Text, cancellationToken);
        return true;
    }

    private async Task DoHandleTextMessage(Chat chat, User from, string text, CancellationToken cancellationToken)
    {
        Logger.LogInformation("New text message in private chat: {ChatId}, userId: {UserId}, isBot: {IsBot}, text: {Text}", chat.Id, from.Id, from.IsBot, text);

        if (text.ToLowerInvariant() == "/start")
        {
            await Butler.UserChat.SayHelloAsync(chat.Id, cancellationToken);
            return;
        }

        var request = await UserRepository.FindJoinRequestAsync(from.Id, cancellationToken);

        if (request == null || request.IsWhoisProvided)
        {
            await Butler.UserChat.SayConfusedAsync(chat.Id, cancellationToken);
            return;
        }

        (var result, var error) = whoisValidator.CheckMessageText(text);
        if (!result)
        {
            await Butler.UserChat.WarnWhoisValidationFailedAsync(chat.Id, error, cancellationToken);
            return;
        }

        var withWhois = request with { Whois = text };
        await UserRepository.UpdateJoinRequestAsync(withWhois, cancellationToken);

        await Butler.UserChat.SayWhoisOkAndAskToRequestAccessAsync(chat.Id, cancellationToken);
    }
}

