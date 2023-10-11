using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Butler.Bot.Core.UserChat;

public class RegisterQueryHandler : UpdateHandlerBase
{
    public RegisterQueryHandler(IButlerBot butler, IUserRepository userRepository, ILogger<RegisterQueryHandler> logger)
        : base(butler, userRepository, logger)
    {
    }
    public override async Task<bool> TryHandleUpdateAsync(Update update, CancellationToken cancellationToken)
    {
        // Only handle callback queries from live messages
        if (update.CallbackQuery == null || update.CallbackQuery.Message == null || update.CallbackQuery.From == null) return false;

        // Only accept callback from private chat
        if (update.CallbackQuery.Message.Chat.Type != ChatType.Private) return false;

        // Only handle [register]
        if (update.CallbackQuery.Data != "register") return false;

        await Butler.StopQuerySpinnerAsync(update.CallbackQuery.Id, cancellationToken);

        await DoHandleRegisterCallbackAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.From.Id, cancellationToken);
        return true;
    }

    private async Task DoHandleRegisterCallbackAsync(long chatId, long userId, CancellationToken cancellationToken)
    {
        Logger.LogInformation("Registration requsted in private chat: {ChatId}", chatId);

        var currentStatus = await Butler.TargetGroup.GetMemberStatusAsync(userId, cancellationToken);

        if (IsAlreadyMember(currentStatus))
        {
            await Butler.UserChat.SayAlreadyMemberAsync(chatId, cancellationToken);
        }
        else if(IsBlocked(currentStatus))
        {
            await Butler.UserChat.SayBlockedAsync(chatId, cancellationToken);
        }
        else
        {
            var request = await UserRepository.FindOrCreateRequestAsync(userId, cancellationToken);

            if (request.IsWhoisProvided)
            {
                await Butler.UserChat.SayUsedToBeMemberAsync(chatId, cancellationToken);
            }
            else
            {
                await Butler.UserChat.AskForWhoisAsync(chatId, cancellationToken);
            }
        }
    }

    private bool IsAlreadyMember(ChatMemberStatus? memberStatus)
    {
        return
            memberStatus == ChatMemberStatus.Creator ||
            memberStatus == ChatMemberStatus.Administrator ||
            memberStatus == ChatMemberStatus.Member ||
            memberStatus == ChatMemberStatus.Restricted;
    }

    private bool IsBlocked(ChatMemberStatus? memberStatus)
    {
        return memberStatus == ChatMemberStatus.Kicked;
    }
}
