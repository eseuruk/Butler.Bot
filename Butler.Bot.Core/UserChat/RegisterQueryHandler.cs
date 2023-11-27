using Butler.Bot.Core.TargetGroup;

namespace Butler.Bot.Core.UserChat;

public class RegisterQueryHandler : IUpdateHandler
{
    private readonly IUserChatBot userChatBot;
    private readonly ITargetGroupBot targetGroupBot;
    private readonly IUserRepository userRepository;

    private readonly ILogger<RegisterQueryHandler> logger;

    public RegisterQueryHandler(IUserChatBot userChatBot, ITargetGroupBot targetGroupBot, IUserRepository userRepository, ILogger<RegisterQueryHandler> logger)
    {
        this.userChatBot = userChatBot;
        this.targetGroupBot = targetGroupBot;
        this.userRepository = userRepository;
        this.logger = logger;
    }

    public async Task<bool> TryHandleUpdateAsync(Update update, CancellationToken cancellationToken)
    {
        // Only handle callback queries from live messages
        if (update.CallbackQuery == null || update.CallbackQuery.Message == null || update.CallbackQuery.From == null) return false;

        // Only accept callback from private chat
        if (update.CallbackQuery.Message.Chat.Type != ChatType.Private) return false;

        // Only handle register requests
        if (update.CallbackQuery.Data == null || !update.CallbackQuery.Data.StartsWith("register")) return false;

        await userChatBot.StopQuerySpinnerAsync(update.CallbackQuery.Id, cancellationToken);

        switch(update.CallbackQuery.Data)
        {
            case "register":
                await DoHandleRegisterAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.From.Id, false, cancellationToken);
                return true;

            case "register-delete":
                await DoHandleRegisterAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.From.Id, true, cancellationToken);
                return true;
        }
        return false;
    }

    private async Task DoHandleRegisterAsync(long chatId, long userId, bool forceDeleteOldWhois, CancellationToken cancellationToken)
    {
        logger.LogInformation("Registration request in private chat: {ChatId}, userId: {UserId}, forceDeleteOldWhois: {ForceDeleteOldWhois}", chatId, userId, forceDeleteOldWhois);

        var chatMember = await targetGroupBot.GetChatMemberAsync(userId, cancellationToken);

        if (chatMember.Status.IsAlreadyMember())
        {
            await userChatBot.SayAlreadyMemberAsync(chatId, cancellationToken);
        }
        else if (chatMember.Status == ChatMemberStatus.Kicked)
        {
            await userChatBot.SayBlockedAsync(chatId, cancellationToken);
        }
        else
        {
            var request = await userRepository.FindOrCreateRequestAsync(userId, cancellationToken);

            if (request.IsWhoisProvided)
            {
                if (forceDeleteOldWhois)
                {
                    await targetGroupBot.TryDeleteMessageAsync(request.WhoisMessageId, cancellationToken);

                    var emptyRequest = request with { Whois = string.Empty, WhoisMessageId = 0, UserChatId = 0 };
                    await userRepository.UpdateJoinRequestAsync(emptyRequest, cancellationToken);

                    await userChatBot.AskForWhoisAsync(chatId, cancellationToken);
                }
                else
                {
                    await userChatBot.SayUsedToBeMemberAsync(chatId, cancellationToken);
                }
            }
            else
            {
                await userChatBot.AskForWhoisAsync(chatId, cancellationToken);
            }
        }
    }
}
