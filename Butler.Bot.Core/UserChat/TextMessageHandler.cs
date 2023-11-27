namespace Butler.Bot.Core.UserChat;

public class TextMessageHandler : IUpdateHandler
{
    private readonly IUserChatBot userChatBot;
    private readonly IUserRepository userRepository;
    private readonly IWhoisValidator whoisValidator;

    private readonly ILogger<TextMessageHandler> logger;

    public TextMessageHandler(IUserChatBot userChatBot, IUserRepository userRepository, IWhoisValidator whoisValidator, ILogger<TextMessageHandler> logger)
    {
        this.userChatBot = userChatBot;
        this.userRepository = userRepository;
        this.whoisValidator = whoisValidator;
        this.logger = logger;
    }

    public async Task<bool> TryHandleUpdateAsync(Update update, CancellationToken cancellationToken)
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
        logger.LogInformation("New text message in private chat: {ChatId}, userId: {UserId}, isBot: {IsBot}, text: {Text}", chat.Id, from.Id, from.IsBot, text);

        bool menu = await TryHandleMenuCommands(chat, from, text, cancellationToken);
        if (menu) return;

        var request = await userRepository.FindJoinRequestAsync(from.Id, cancellationToken);

        if (request == null || request.IsWhoisProvided)
        {
            await userChatBot.SayConfusedAsync(chat.Id, cancellationToken);
            return;
        }

        (var result, var error) = whoisValidator.CheckMessageText(text);
        if (!result)
        {
            await userChatBot.WarnWhoisValidationFailedAsync(chat.Id, error, cancellationToken);
            return;
        }

        var withWhois = request with { Whois = text };
        await userRepository.UpdateJoinRequestAsync(withWhois, cancellationToken);

        await userChatBot.SayWhoisOkAndAskToRequestAccessAsync(chat.Id, cancellationToken);
    }

    private async Task<bool> TryHandleMenuCommands(Chat chat, User from, string text, CancellationToken cancellationToken)
    {
        switch (text.ToLowerInvariant())
        {
            case "/start":
                await userChatBot.SayHelloAsync(chat.Id, cancellationToken);
                return true;

            case "/leave":
                await userChatBot.ShowLeaveRequestAsync(chat.Id, cancellationToken);
                return true;

            case "/version":
                await userChatBot.ShowBotVersionAsync(chat.Id, cancellationToken);
                return true;
        }

        return false;
    }

}
