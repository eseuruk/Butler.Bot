using Butler.Bot.Core.TargetGroup;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

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

        // Only handle [register]
        if (update.CallbackQuery.Data != "register") return false;

        await userChatBot.StopQuerySpinnerAsync(update.CallbackQuery.Id, cancellationToken);

        await DoHandleRegisterCallbackAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.From.Id, cancellationToken);
        return true;
    }

    private async Task DoHandleRegisterCallbackAsync(long chatId, long userId, CancellationToken cancellationToken)
    {
        logger.LogInformation("Registration requested in private chat: {ChatId}", chatId);

        var chatMember = await targetGroupBot.GetChatMemberAsync(userId, cancellationToken);

        if (IsAlreadyMember(chatMember.Status))
        {
            await userChatBot.SayAlreadyMemberAsync(chatId, cancellationToken);
        }
        else if(IsBlocked(chatMember.Status))
        {
            await userChatBot.SayBlockedAsync(chatId, cancellationToken);
        }
        else
        {
            var request = await userRepository.FindOrCreateRequestAsync(userId, cancellationToken);

            if (request.IsWhoisProvided)
            {
                await userChatBot.SayUsedToBeMemberAsync(chatId, cancellationToken);
            }
            else
            {
                await userChatBot.AskForWhoisAsync(chatId, cancellationToken);
            }
        }
    }

    private bool IsAlreadyMember(ChatMemberStatus memberStatus)
    {
        return
            memberStatus == ChatMemberStatus.Creator ||
            memberStatus == ChatMemberStatus.Administrator ||
            memberStatus == ChatMemberStatus.Member ||
            memberStatus == ChatMemberStatus.Restricted;
    }

    private bool IsBlocked(ChatMemberStatus memberStatus)
    {
        return memberStatus == ChatMemberStatus.Kicked;
    }
}
