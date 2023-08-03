using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;

namespace Butler.Bot.Core.AdminGroup;

public partial class ReviewWhoisHandler : UpdateHandlerBase
{
    public ReviewWhoisHandler(ButlerBot butler, IUserRepository userRepository, ILogger<ReviewWhoisHandler> logger)
        : base(butler, userRepository, logger)
    {
    }
    public override async Task<bool> TryHandleUpdateAsync(Update update, CancellationToken cancellationToken)
    {
        // Only handle callback queries from live messages
        if (update.CallbackQuery == null || update.CallbackQuery.Message == null) return false;

        // Only handle requests from admin group
        if (update.CallbackQuery.Message.Chat.Id != Butler.Options.AdminGroupId) return false;

        if (!ReviewQueryData.TryParse(update.CallbackQuery.Data, out var callBackData)) return false;

        switch(callBackData.Operation)
        {
            case "prejoin-approve":
                await Butler.StopQuerySpinnerAsync(update.CallbackQuery.Id, cancellationToken);
                await DoHandlePreJoinApproveAsync(update.CallbackQuery.From, callBackData.UserId, update.CallbackQuery.Message.MessageId, cancellationToken);
                return true;

            case "prejoin-decline":
                await Butler.StopQuerySpinnerAsync(update.CallbackQuery.Id, cancellationToken);
                await DoHandlePreJoinDeclineAsync(update.CallbackQuery.From, callBackData.UserId, update.CallbackQuery.Message.MessageId, cancellationToken);
                return true;

            case "postjoin-delete":
                await Butler.StopQuerySpinnerAsync(update.CallbackQuery.Id, cancellationToken);
                await DoHandlePostJoinDeleteAsync(update.CallbackQuery.From, callBackData.UserId, update.CallbackQuery.Message.MessageId, cancellationToken);
                return true;

            default: return false;
        }
    }

    private async Task DoHandlePreJoinApproveAsync(User admin, long userId, int messageId, CancellationToken cancellationToken)
    {
        Logger.LogInformation("Whois approve request in admin group: {AdminGroup}, admin: {AdminId}, userID: {UserId}", Butler.Options.AdminGroupId, admin.Id, userId);

        bool alreadyAdded = await Butler.TargetGroup.IsAreadyMemberAsync(userId, cancellationToken);
        if (alreadyAdded) return;

        var originalRequest = await UserRepository.FindJoinRequestAsync(userId, cancellationToken);
        if (originalRequest == null) return;

        await Butler.TargetGroup.ApproveJoinRequestAsync(userId, cancellationToken);

        await Butler.AdminGroup.MarkJoinRequestAsApprovedAsync(messageId, admin, cancellationToken);

        if (originalRequest.IsUserChatIdSaved)
        {
            // Nothing to say here. Message will be sent after user is added to the group
        }
    }

    private async Task DoHandlePreJoinDeclineAsync(User admin, long userId, int messageId, CancellationToken cancellationToken)
    {
        Logger.LogInformation("Whois decline request in admin group: {AdminGroup}, admin: {AdminId}, userID: {UserId}", Butler.Options.AdminGroupId, admin.Id, userId);

        bool alreadyAdded = await Butler.TargetGroup.IsAreadyMemberAsync(userId, cancellationToken);
        if (alreadyAdded) return;

        var originalRequest = await UserRepository.FindJoinRequestAsync(userId, cancellationToken);
        if (originalRequest == null) return;

        await Butler.TargetGroup.DeclineJoinRequestAsync(userId, cancellationToken);

        var withoutWhois = originalRequest with { Whois = string.Empty };
        await UserRepository.UpdateJoinRequestAsync(withoutWhois, cancellationToken);

        await Butler.AdminGroup.MarkJoinRequestAsDeclinedAsync(messageId, admin, cancellationToken);

        if (originalRequest.IsUserChatIdSaved)
        {
            await Butler.UserChat.TrySayingRequestDeclinedAsync(originalRequest.UserChatId, cancellationToken);
        }
    }

    private async Task DoHandlePostJoinDeleteAsync(User admin, long userId, int messageId, CancellationToken cancellationToken)
    {
        Logger.LogInformation("User delete request in admin group: {AdminGroup}, admin: {AdminId}, userID: {UserId}", Butler.Options.AdminGroupId, admin.Id, userId);

        bool isMember = await Butler.TargetGroup.IsAreadyMemberAsync(userId, cancellationToken);
        if (!isMember) return;

        var originalRequest = await UserRepository.FindJoinRequestAsync(userId, cancellationToken);
        if (originalRequest == null) return;

        await Butler.TargetGroup.DeleteUserAsync(userId, cancellationToken);

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
}

