using Butler.Bot.Core.TargetGroup;
using Butler.Bot.Core.UserChat;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Butler.Bot.Core.AdminGroup;

public class ReviewWhoisHandler : IUpdateHandler
{
    private readonly ButlerOptions options;
    private readonly IUserChatBot userChatBot;
    private readonly ITargetGroupBot targetGroupBot;
    private readonly IAdminGroupBot adminGroupBot;
    private readonly IUserRepository userRepository;
    private readonly IInlineStateManager inlineStateManager;

    private readonly ILogger<ReviewWhoisHandler> logger;

    public ReviewWhoisHandler(IOptions<ButlerOptions> options, IUserChatBot userChatBot, ITargetGroupBot targetGroupBot, IAdminGroupBot adminGroupBot, IUserRepository userRepository, IInlineStateManager inlineStateManager, ILogger<ReviewWhoisHandler> logger)
    {
        this.options = options.Value;
        this.userChatBot = userChatBot;
        this.targetGroupBot = targetGroupBot;
        this.adminGroupBot = adminGroupBot;
        this.userRepository = userRepository;
        this.inlineStateManager = inlineStateManager;
        this.logger = logger;
    }

    public async Task<bool> TryHandleUpdateAsync(Update update, CancellationToken cancellationToken)
    {
        // Only handle callback queries from live messages
        if (update.CallbackQuery == null || update.CallbackQuery.Message == null) return false;

        // Only handle requests from admin group
        if (update.CallbackQuery.Message.Chat.Id != options.AdminGroupId) return false;

        if (update.CallbackQuery.Data == "prejoin-approve")
        {
            await adminGroupBot.StopQuerySpinnerAsync(update.CallbackQuery.Id, cancellationToken);
            var user = inlineStateManager.GetStateFromMessage<User>(update.CallbackQuery.Message);
            await DoHandlePreJoinApproveAsync(update.CallbackQuery.From, user, update.CallbackQuery.Message.MessageId, cancellationToken);
            return true;
        }
        else if (update.CallbackQuery.Data == "prejoin-decline")
        {
            await adminGroupBot.StopQuerySpinnerAsync(update.CallbackQuery.Id, cancellationToken);
            var user = inlineStateManager.GetStateFromMessage<User>(update.CallbackQuery.Message);
            await DoHandlePreJoinDeclineAsync(update.CallbackQuery.From, user, update.CallbackQuery.Message.MessageId, cancellationToken);
            return true;
        }
        else if (update.CallbackQuery.Data == "postjoin-delete")
        {
            await adminGroupBot.StopQuerySpinnerAsync(update.CallbackQuery.Id, cancellationToken);
            var user = inlineStateManager.GetStateFromMessage<User>(update.CallbackQuery.Message);
            await DoHandlePostJoinDeleteAsync(update.CallbackQuery.From, user, update.CallbackQuery.Message.MessageId, cancellationToken);
            return true;
        }
            
        return false;
    }

    private async Task DoHandlePreJoinApproveAsync(User admin, User user, int messageId, CancellationToken cancellationToken)
    {
        logger.LogInformation("Whois approve request in admin group: {AdminGroup}, admin: {AdminId}, userID: {UserId}", options.AdminGroupId, admin.Id, user.Id);

        var currentStatus = await targetGroupBot.GetMemberStatusAsync(user.Id, cancellationToken);
        if (!CanBeAddedToChat(currentStatus)) return;

        var originalRequest = await userRepository.FindJoinRequestAsync(user.Id, cancellationToken);
        if (originalRequest == null) return;

        await targetGroupBot.ApproveJoinRequestAsync(user.Id, cancellationToken);

        await adminGroupBot.MarkJoinRequestAsApprovedAsync(messageId, admin, cancellationToken);

        if (originalRequest.IsUserChatIdSaved)
        {
            // Nothing to say here. Message will be sent after user is added to the group
        }
    }

    private async Task DoHandlePreJoinDeclineAsync(User admin, User user, int messageId, CancellationToken cancellationToken)
    {
        logger.LogInformation("Whois decline request in admin group: {AdminGroup}, admin: {AdminId}, userID: {UserId}", options.AdminGroupId, admin.Id, user.Id);

        var currentStatus = await targetGroupBot.GetMemberStatusAsync(user.Id, cancellationToken);
        if (!CanBeAddedToChat(currentStatus)) return;

        var originalRequest = await userRepository.FindJoinRequestAsync(user.Id, cancellationToken);
        if (originalRequest == null) return;

        await targetGroupBot.DeclineJoinRequestAsync(user.Id, cancellationToken);

        var withoutWhois = originalRequest with { Whois = string.Empty };
        await userRepository.UpdateJoinRequestAsync(withoutWhois, cancellationToken);

        await adminGroupBot.MarkJoinRequestAsDeclinedAsync(messageId, admin, cancellationToken);

        if (originalRequest.IsUserChatIdSaved)
        {
            await userChatBot.TrySayingRequestDeclinedAsync(originalRequest.UserChatId, cancellationToken);
        }
    }

    private async Task DoHandlePostJoinDeleteAsync(User admin, User user, int messageId, CancellationToken cancellationToken)
    {
        logger.LogInformation("User delete request in admin group: {AdminGroup}, admin: {AdminId}, userID: {UserId}", options.AdminGroupId, admin.Id, user.Id);

        var currentStatus = await targetGroupBot.GetMemberStatusAsync(user.Id, cancellationToken);
        if (!CanBeDeletedFromChat(currentStatus)) return;

        var originalRequest = await userRepository.FindJoinRequestAsync(user.Id, cancellationToken);
        if (originalRequest == null) return;

        await targetGroupBot.SayLeavingToChangeWhoisAsync(user, cancellationToken);

        await targetGroupBot.DeleteUserAsync(user.Id, cancellationToken);

        if (originalRequest.IsWhoisMessageWritten)
        {
            await targetGroupBot.DeleteMessageAsync(originalRequest.WhoisMessageId, cancellationToken);
        }

        var emptyRequest = originalRequest with { Whois = string.Empty, WhoisMessageId = 0, UserChatId = 0 };
        await userRepository.UpdateJoinRequestAsync(emptyRequest, cancellationToken);

        await adminGroupBot.MarkUserAsDeletedAsync(messageId, admin, cancellationToken);
        
        if (originalRequest.IsUserChatIdSaved)
        {
            await userChatBot.TrySayingUserDeletedAsync(originalRequest.UserChatId, cancellationToken);
        }
    }

    private bool CanBeAddedToChat(ChatMemberStatus? memberStatus)
    {
        return 
            memberStatus == null ||                 // never joinned
            memberStatus == ChatMemberStatus.Left;  // left but not blocked
    }

    private bool CanBeDeletedFromChat(ChatMemberStatus? memberStatus)
    {
        return
            memberStatus == ChatMemberStatus.Creator ||
            memberStatus == ChatMemberStatus.Administrator ||
            memberStatus == ChatMemberStatus.Member ||
            memberStatus == ChatMemberStatus.Restricted;
    }
}

