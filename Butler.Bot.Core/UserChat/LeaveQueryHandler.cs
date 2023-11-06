using Butler.Bot.Core.TargetGroup;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Butler.Bot.Core.UserChat;

public class LeaveQueryHandler : IUpdateHandler
{
    private readonly IUserChatBot userChatBot;
    private readonly ITargetGroupBot targetGroupBot;
    private readonly IUserRepository userRepository;

    private readonly ILogger<RegisterQueryHandler> logger;

    public LeaveQueryHandler(IUserChatBot userChatBot, ITargetGroupBot targetGroupBot, IUserRepository userRepository, ILogger<RegisterQueryHandler> logger)
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

        // Only handle leave requests
        if (update.CallbackQuery.Data == null || !update.CallbackQuery.Data.StartsWith("leave-")) return false;

        await userChatBot.StopQuerySpinnerAsync(update.CallbackQuery.Id, cancellationToken);

        switch (update.CallbackQuery.Data)
        {
            case "leave-confirm":
                await DoHandleLeaveConfirmedAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.From.Id, update.CallbackQuery.Message.MessageId, cancellationToken);
                return true;
            case "leave-cancel":
                await DoHandleLeaveCanceledAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.From.Id, update.CallbackQuery.Message.MessageId, cancellationToken);
                return true;
        }
        
        return true;
    }

    private async Task DoHandleLeaveConfirmedAsync(long chatId, long userId, int messageId, CancellationToken cancellationToken)
    {
        logger.LogInformation("Leave request confirmed in private chat: {ChatId}, userId: {UserId}", chatId, userId);

        var joinRequest = await userRepository.FindJoinRequestAsync(userId, cancellationToken);
        var chatMember = await targetGroupBot.GetChatMemberAsync(userId, cancellationToken);

        if ( joinRequest is not null)
        {
            await userRepository.DeleteJoinRequestAsync(userId, cancellationToken);
        }
        else
        {
            logger.LogInformation("Join request is not found so nothing to delete. UserId: {UserId}", userId);
        }

        if (chatMember.Status.IsRemovableMember())
        {
            await targetGroupBot.DeleteUserAsync(userId, cancellationToken);
        }
        else
        {
            logger.LogInformation("User is not removable from target group. UserId: {UserId}, userStatus: {UserStatus}", userId, chatMember.Status);
        }

        await userChatBot.SayLeaveRequestFulfilledAsync(chatId, messageId, cancellationToken);
    }

    private async Task DoHandleLeaveCanceledAsync(long chatId, long userId, int messageId, CancellationToken cancellationToken)
    {
        logger.LogInformation("Leave request canceled in private chat: {ChatId}, userId: {UserId}", chatId, userId);

        await userChatBot.SayLeaveRequestCanceledAsync(chatId, messageId, cancellationToken);
    }
}
