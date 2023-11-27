using Butler.Bot.Core.TargetGroup;
using Butler.Bot.Core.UserChat;

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

        (var joinRequestToDelete, var userToDelete) = await DecideWhatToDeleteAsync(inlineState, cancellationToken);

        // 1. delete whois message from target group
        await targetGroupBot.TryDeleteMessageAsync(inlineState.WhoisMessageId, cancellationToken);

        if (joinRequestToDelete != null)
        {
            // 2. delete join request from db
            var emptyRequest = joinRequestToDelete with { Whois = string.Empty, WhoisMessageId = 0, UserChatId = 0 };
            await userRepository.UpdateJoinRequestAsync(emptyRequest, cancellationToken);
        }

        if (userToDelete != null)
        {
            // 3. delete user from the target group
            await targetGroupBot.SayLeavingToChangeWhoisAsync(userToDelete, cancellationToken);
            await targetGroupBot.DeleteUserAsync(userToDelete.Id, cancellationToken);

            // 4. notify user about deletion 
            if (joinRequestToDelete != null && joinRequestToDelete.IsWhoisMessageWritten)
            {
                await userChatBot.TrySayingUserDeletedAsync(joinRequestToDelete.UserChatId, cancellationToken);
            }
        }

        await adminGroupBot.MarkUserAsDeletedAsync(messageId, admin, cancellationToken);
    }

    private async Task DoHandlePostJoinDeleteCanceledAsync(User admin, PostJoinInlineState inlineState, int messageId, CancellationToken cancellationToken)
    {
        logger.LogInformation("User delete request canceled in admin group: {AdminGroup}, admin: {AdminId}, userID: {UserId}, whoisMessageId: {WhoisMessageId}", options.AdminGroupId, admin.Id, inlineState.UserId, inlineState.WhoisMessageId);

        await adminGroupBot.CancelUserDeletionAsync(messageId, cancellationToken);
    }

    private async Task<(JoinRequest?, User?)> DecideWhatToDeleteAsync(PostJoinInlineState inlineState, CancellationToken cancellationToken)
    {
        var joinRequest = await userRepository.FindJoinRequestAsync(inlineState.UserId, cancellationToken);
        var chatMember = await targetGroupBot.GetChatMemberAsync(inlineState.UserId, cancellationToken);

        string caseDescription, decision;

        JoinRequest? requestToDelete;
        User? userToDelete;

        if (joinRequest == null)
        {
            if (chatMember.Status.IsRemovableMember())
            {
                caseDescription = "request was deleted previously, but user still a member of the group somehow";
                decision = "removing user from the group";

                requestToDelete = null;
                userToDelete = chatMember.User;
            }
            else
            {
                caseDescription = "request was deleted previously, and user can not be removed from the group";
                decision = "nothing to delete";

                requestToDelete = null;
                userToDelete = null;
            }
        }
        else
        {
            if (joinRequest.WhoisMessageId != inlineState.WhoisMessageId)
            {
                caseDescription = "request exists, but admin clicked not on the latest whois message";
                decision = "nothing to delete. admin needs to click on another whois message to remove the user";

                requestToDelete = null;
                userToDelete = null;
            }
            else if (chatMember.Status.IsRemovableMember())
            {
                caseDescription = "request exists, user is member of the group";
                decision = "removing request and user from the group";

                requestToDelete = joinRequest;
                userToDelete = chatMember.User;
            }
            else
            {
                caseDescription = "request exists, but user can not be removed frmom the group";
                decision = "removing request";

                requestToDelete = joinRequest;
                userToDelete = null;
            }
        }

        logger.LogInformation("Decide what to delete. UserId: {UserId}, Status: {Status}, WhoisMessageId: {WhoisMessageId}, Case: {Case}, Result: {Result}", inlineState.UserId, chatMember.Status, inlineState.WhoisMessageId, caseDescription, decision);
        return (requestToDelete, userToDelete);
    }
}

