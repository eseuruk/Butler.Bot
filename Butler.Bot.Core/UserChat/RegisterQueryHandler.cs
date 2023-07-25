using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Butler.Bot.Core.UserChat;

public class RegisterQueryHandler : UpdateHandlerBase
{
    public RegisterQueryHandler(ButlerBot butler, IUserRepository userRepository, ILoggerFactory loggerFactory)
        : base(butler, userRepository, loggerFactory.CreateLogger<RegisterQueryHandler>())
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

        if (await Butler.TargetGroup.IsAreadyMemberAsync(userId, cancellationToken))
        {
            await Butler.UserChat.SayAlreadyMemberAsync(chatId, cancellationToken);
        }
        else
        {
            var request = await UserRepository.FindOrCreateRequest(userId);

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
}
