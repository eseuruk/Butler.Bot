using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Butler.Bot.Core.AdminGroup;

public partial class ReviewWhoisHandler : UpdateHandlerBase
{
    private readonly InlineStateManager inlineStateManager;

    public ReviewWhoisHandler(IButlerBot butler, IUserRepository userRepository, InlineStateManager inlineStateManager, ILogger<ReviewWhoisHandler> logger)
        : base(butler, userRepository, logger)
    {
        this.inlineStateManager = inlineStateManager;
    }

    public override async Task<bool> TryHandleUpdateAsync(Update update, CancellationToken cancellationToken)
    {
        // Only handle callback queries from live messages
        if (update.CallbackQuery == null || update.CallbackQuery.Message == null) return false;

        // Only handle requests from admin group
        if (update.CallbackQuery.Message.Chat.Id != Butler.Options.AdminGroupId) return false;

        if (update.CallbackQuery.Data == "prejoin-approve")
        {
            await Butler.StopQuerySpinnerAsync(update.CallbackQuery.Id, cancellationToken);
            var user = inlineStateManager.GetStateFromMessage<User>(update.CallbackQuery.Message);
            await DoHandlePreJoinApproveAsync(update.CallbackQuery.From, user, update.CallbackQuery.Message.MessageId, cancellationToken);
            return true;
        }
        else if (update.CallbackQuery.Data == "prejoin-decline")
        {
            await Butler.StopQuerySpinnerAsync(update.CallbackQuery.Id, cancellationToken);
            var user = inlineStateManager.GetStateFromMessage<User>(update.CallbackQuery.Message);
            await DoHandlePreJoinDeclineAsync(update.CallbackQuery.From, user, update.CallbackQuery.Message.MessageId, cancellationToken);
            return true;
        }
        else if (update.CallbackQuery.Data == "postjoin-delete")
        {
            await Butler.StopQuerySpinnerAsync(update.CallbackQuery.Id, cancellationToken);
            var user = inlineStateManager.GetStateFromMessage<User>(update.CallbackQuery.Message);
            await DoHandlePostJoinDeleteAsync(update.CallbackQuery.From, user, update.CallbackQuery.Message.MessageId, cancellationToken);
            return true;
        }
            
        return false;
    }

    private async Task DoHandlePreJoinApproveAsync(User admin, User user, int messageId, CancellationToken cancellationToken)
    {
        Logger.LogInformation("Whois approve request in admin group: {AdminGroup}, admin: {AdminId}, userID: {UserId}", Butler.Options.AdminGroupId, admin.Id, user.Id);

        var currentStatus = await Butler.TargetGroup.GetMemberStatusAsync(user.Id, cancellationToken);
        if (!CanBeAddedToChat(currentStatus)) return;

        var originalRequest = await UserRepository.FindJoinRequestAsync(user.Id, cancellationToken);
        if (originalRequest == null) return;

        await Butler.TargetGroup.ApproveJoinRequestAsync(user.Id, cancellationToken);

        await Butler.AdminGroup.MarkJoinRequestAsApprovedAsync(messageId, admin, cancellationToken);

        if (originalRequest.IsUserChatIdSaved)
        {
            // Nothing to say here. Message will be sent after user is added to the group
        }
    }

    private async Task DoHandlePreJoinDeclineAsync(User admin, User user, int messageId, CancellationToken cancellationToken)
    {
        Logger.LogInformation("Whois decline request in admin group: {AdminGroup}, admin: {AdminId}, userID: {UserId}", Butler.Options.AdminGroupId, admin.Id, user.Id);

        var currentStatus = await Butler.TargetGroup.GetMemberStatusAsync(user.Id, cancellationToken);
        if (!CanBeAddedToChat(currentStatus)) return;

        var originalRequest = await UserRepository.FindJoinRequestAsync(user.Id, cancellationToken);
        if (originalRequest == null) return;

        await Butler.TargetGroup.DeclineJoinRequestAsync(user.Id, cancellationToken);

        var withoutWhois = originalRequest with { Whois = string.Empty };
        await UserRepository.UpdateJoinRequestAsync(withoutWhois, cancellationToken);

        await Butler.AdminGroup.MarkJoinRequestAsDeclinedAsync(messageId, admin, cancellationToken);

        if (originalRequest.IsUserChatIdSaved)
        {
            await Butler.UserChat.TrySayingRequestDeclinedAsync(originalRequest.UserChatId, cancellationToken);
        }
    }

    private async Task DoHandlePostJoinDeleteAsync(User admin, User user, int messageId, CancellationToken cancellationToken)
    {
        Logger.LogInformation("User delete request in admin group: {AdminGroup}, admin: {AdminId}, userID: {UserId}", Butler.Options.AdminGroupId, admin.Id, user.Id);

        var currentStatus = await Butler.TargetGroup.GetMemberStatusAsync(user.Id, cancellationToken);
        if (!CanBeDeletedFromChat(currentStatus)) return;

        var originalRequest = await UserRepository.FindJoinRequestAsync(user.Id, cancellationToken);
        if (originalRequest == null) return;

        await Butler.TargetGroup.SayLeavingToChangeWhoisAsync(user, cancellationToken);

        await Butler.TargetGroup.DeleteUserAsync(user.Id, cancellationToken);

        if (originalRequest.IsWhoisMessageWritten)
        {
            await Butler.TargetGroup.DeleteMessageAsync(originalRequest.WhoisMessageId, cancellationToken);
        }

        var emptyRequest = originalRequest with { Whois = string.Empty, WhoisMessageId = 0, UserChatId = 0 };
        await UserRepository.UpdateJoinRequestAsync(emptyRequest, cancellationToken);

        await Butler.AdminGroup.MarkUserAsDeletedAsync(messageId, admin, cancellationToken);
        
        if (originalRequest.IsUserChatIdSaved)
        {
            await Butler.UserChat.TrySayingUserDeletedAsync(originalRequest.UserChatId, cancellationToken);
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

