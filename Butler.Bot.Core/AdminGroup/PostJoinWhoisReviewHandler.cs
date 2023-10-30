using Butler.Bot.Core.TargetGroup;
using Butler.Bot.Core.UserChat;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Butler.Bot.Core.AdminGroup;

public class PostJoinWhoisReviewHandler : IUpdateHandler
{
    private readonly ButlerOptions options;
    private readonly IUserChatBot userChatBot;
    private readonly ITargetGroupBot targetGroupBot;
    private readonly IAdminGroupBot adminGroupBot;
    private readonly IUserRepository userRepository;
    private readonly IInlineStateManager inlineStateManager;

    private readonly ILogger<PostJoinWhoisReviewHandler> logger;

    public PostJoinWhoisReviewHandler(IOptions<ButlerOptions> options, IUserChatBot userChatBot, ITargetGroupBot targetGroupBot, IAdminGroupBot adminGroupBot, IUserRepository userRepository, IInlineStateManager inlineStateManager, ILogger<PostJoinWhoisReviewHandler> logger)
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

        // Only postjoin review requests
        if (update.CallbackQuery.Data == null || !update.CallbackQuery.Data.StartsWith("postjoin-")) return false;

        await adminGroupBot.StopQuerySpinnerAsync(update.CallbackQuery.Id, cancellationToken);
        var inlineState = inlineStateManager.GetStateFromMessage<PostJoinInlineState>(update.CallbackQuery.Message);

        switch (update.CallbackQuery.Data)
        { 
        case "postjoin-delete":
            await DoHandlePostJoinDeleteAsync(update.CallbackQuery.From, inlineState, update.CallbackQuery.Message.MessageId, cancellationToken);
            return true;
        case "postjoin-delete-confirm":
            await DoHandlePostJoinDeleteConfirmedAsync(update.CallbackQuery.From, inlineState, update.CallbackQuery.Message.MessageId, cancellationToken);
            return true;
        case "postjoin-delete-cancel":
            await DoHandlePostJoinDeleteCanceledAsync(update.CallbackQuery.From, inlineState, update.CallbackQuery.Message.MessageId, cancellationToken);
            return true;
        case "postjoin-deleted":
            return true;
        }

        return false;
    }

    private async Task DoHandlePostJoinDeleteAsync(User admin, PostJoinInlineState inlineState, int messageId, CancellationToken cancellationToken)
    {
        logger.LogInformation("User delete request in admin group: {AdminGroup}, admin: {AdminId}, userID: {UserId}, whoisMessageId: {WhoisMessageId}", options.AdminGroupId, admin.Id, inlineState.UserId, inlineState.WhoisMessageId);

        await adminGroupBot.AskForUserDeleteConfirmationAsync(messageId, cancellationToken);
    }

    private async Task DoHandlePostJoinDeleteConfirmedAsync(User admin, PostJoinInlineState inlineState, int messageId, CancellationToken cancellationToken)
    {
        logger.LogInformation("User delete request confirmed in admin group: {AdminGroup}, admin: {AdminId}, userID: {UserId}, whoisMessageId: {WhoisMessageId}", options.AdminGroupId, admin.Id, inlineState.UserId, inlineState.WhoisMessageId);

        var chatMember = await targetGroupBot.GetChatMemberAsync(inlineState.UserId, cancellationToken);
        if (!CanBeDeletedFromChat(chatMember.Status)) return;

        var originalRequest = await userRepository.FindJoinRequestAsync(inlineState.UserId, cancellationToken);
        if (originalRequest == null || originalRequest.WhoisMessageId != inlineState.WhoisMessageId) return;

        await targetGroupBot.SayLeavingToChangeWhoisAsync(chatMember.User, cancellationToken);

        await targetGroupBot.DeleteUserAsync(inlineState.UserId, cancellationToken);

        if (originalRequest.IsWhoisMessageWritten)
        {
            await targetGroupBot.TryDeleteMessageAsync(originalRequest.WhoisMessageId, cancellationToken);
        }

        var emptyRequest = originalRequest with { Whois = string.Empty, WhoisMessageId = 0, UserChatId = 0 };
        await userRepository.UpdateJoinRequestAsync(emptyRequest, cancellationToken);

        await adminGroupBot.MarkUserAsDeletedAsync(messageId, admin, cancellationToken);
        
        if (originalRequest.IsUserChatIdSaved)
        {
            await userChatBot.TrySayingUserDeletedAsync(originalRequest.UserChatId, cancellationToken);
        }
    }
    private async Task DoHandlePostJoinDeleteCanceledAsync(User admin, PostJoinInlineState inlineState, int messageId, CancellationToken cancellationToken)
    {
        logger.LogInformation("User delete request canceled in admin group: {AdminGroup}, admin: {AdminId}, userID: {UserId}, whoisMessageId: {WhoisMessageId}", options.AdminGroupId, admin.Id, inlineState.UserId, inlineState.WhoisMessageId);

        await adminGroupBot.CancelUserDeletionAsync(messageId, cancellationToken);
    }

    private bool CanBeDeletedFromChat(ChatMemberStatus memberStatus)
    {
        return
            memberStatus == ChatMemberStatus.Creator ||
            memberStatus == ChatMemberStatus.Administrator ||
            memberStatus == ChatMemberStatus.Member ||
            memberStatus == ChatMemberStatus.Restricted;
    }
}

