using Butler.Bot.Core.TargetGroup;
using Butler.Bot.Core.UserChat;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Butler.Bot.Core.AdminGroup;

public class PreJoinWhoisReviewHandler : IUpdateHandler
{
    private readonly ButlerOptions options;
    private readonly IUserChatBot userChatBot;
    private readonly ITargetGroupBot targetGroupBot;
    private readonly IAdminGroupBot adminGroupBot;
    private readonly IUserRepository userRepository;
    private readonly IInlineStateManager inlineStateManager;

    private readonly ILogger<PreJoinWhoisReviewHandler> logger;

    public PreJoinWhoisReviewHandler(IOptions<ButlerOptions> options, IUserChatBot userChatBot, ITargetGroupBot targetGroupBot, IAdminGroupBot adminGroupBot, IUserRepository userRepository, IInlineStateManager inlineStateManager, ILogger<PreJoinWhoisReviewHandler> logger)
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

        // Only prejoin review requests
        if (update.CallbackQuery.Data == null || !update.CallbackQuery.Data.StartsWith("prejoin-")) return false;

        await adminGroupBot.StopQuerySpinnerAsync(update.CallbackQuery.Id, cancellationToken);
        var inlineState = inlineStateManager.GetStateFromMessage<PreJoinInlineState>(update.CallbackQuery.Message);

        switch (update.CallbackQuery.Data)
        {
        case "prejoin-approve":
            await DoHandlePreJoinApproveAsync(update.CallbackQuery.From, inlineState, update.CallbackQuery.Message.MessageId, cancellationToken);
            return true;
        case "prejoin-decline":
            await DoHandlePreJoinDeclineAsync(update.CallbackQuery.From, inlineState, update.CallbackQuery.Message.MessageId, cancellationToken);
            return true;
        case "prejoin-done":
            return true;
        }

        return false;
    }

    private async Task DoHandlePreJoinApproveAsync(User admin, PreJoinInlineState inlineState, int messageId, CancellationToken cancellationToken)
    {
        logger.LogInformation("Whois approve request in admin group: {AdminGroup}, admin: {AdminId}, userID: {UserId}", options.AdminGroupId, admin.Id, inlineState.UserId);

        var chatMember = await targetGroupBot.GetChatMemberAsync(inlineState.UserId, cancellationToken);
        if (!CanBeAddedToChat(chatMember.Status)) return;

        var originalRequest = await userRepository.FindJoinRequestAsync(inlineState.UserId, cancellationToken);
        if (originalRequest == null) return;

        await targetGroupBot.ApproveJoinRequestAsync(inlineState.UserId, cancellationToken);

        await adminGroupBot.MarkJoinRequestAsApprovedAsync(messageId, admin, cancellationToken);

        if (originalRequest.IsUserChatIdSaved)
        {
            // Nothing to say here. Message will be sent after user is added to the group
        }
    }

    private async Task DoHandlePreJoinDeclineAsync(User admin, PreJoinInlineState inlineState, int messageId, CancellationToken cancellationToken)
    {
        logger.LogInformation("Whois decline request in admin group: {AdminGroup}, admin: {AdminId}, userID: {UserId}", options.AdminGroupId, admin.Id, inlineState.UserId);

        var chatMember = await targetGroupBot.GetChatMemberAsync(inlineState.UserId, cancellationToken);
        if (!CanBeAddedToChat(chatMember.Status)) return;

        var originalRequest = await userRepository.FindJoinRequestAsync(inlineState.UserId, cancellationToken);
        if (originalRequest == null) return;

        await targetGroupBot.DeclineJoinRequestAsync(inlineState.UserId, cancellationToken);

        var withoutWhois = originalRequest with { Whois = string.Empty };
        await userRepository.UpdateJoinRequestAsync(withoutWhois, cancellationToken);

        await adminGroupBot.MarkJoinRequestAsDeclinedAsync(messageId, admin, cancellationToken);

        if (originalRequest.IsUserChatIdSaved)
        {
            await userChatBot.TrySayingRequestDeclinedAsync(originalRequest.UserChatId, cancellationToken);
        }
    }

    private bool CanBeAddedToChat(ChatMemberStatus memberStatus)
    {
        return memberStatus == ChatMemberStatus.Left;  // never joinned OR left but not blocked
    }
}

