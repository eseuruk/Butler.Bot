using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;

namespace Butler.Bot.Core.TargetGroup;

public class JoinRequestHandler : UpdateHandlerBase
{
    public JoinRequestHandler(ButlerBot butler, IUserRepository userRepository, ILogger<JoinRequestHandler> logger)
        : base(butler, userRepository, logger)
    {}

    public override async Task<bool> TryHandleUpdateAsync(Update update, CancellationToken cancellationToken)
    {
        // Only handle chat join requests
        if (update.ChatJoinRequest == null) return false;

        // Only handle requests from target group
        if (update.ChatJoinRequest.Chat.Id != Butler.Options.TargetGroupId) return false;

        // Only handle requests with invite link
        if (update.ChatJoinRequest.InviteLink == null) return false;

        // Only handle requests with our dedicated invite link
        if (!CheckInviteLinkOwnership(update.ChatJoinRequest.InviteLink)) return false;

        await DoHandleChatJoinRequestAsync(update.ChatJoinRequest.From, update.ChatJoinRequest.UserChatId, cancellationToken);
        return true;
    }

    private bool CheckInviteLinkOwnership(ChatInviteLink inviteLink)
    {
        if (string.IsNullOrEmpty(Butler.Options.InvitationLinkName))
        {
            Logger.LogInformation("Invite link ownership check is off for target group: {TargetGroupId}, inviteLink: {InviteLink}", Butler.Options.TargetGroupId, inviteLink.InviteLink);
            return true;
        }
        else if (Butler.Options.InvitationLinkName.Equals(inviteLink.Name, StringComparison.OrdinalIgnoreCase))
        {
            Logger.LogInformation("Invite link is recognised for target group: {TargetGroupId}, inviteLinkName: {InviteLinkName}, inviteLink: {InviteLink}", Butler.Options.TargetGroupId, inviteLink.Name, inviteLink.InviteLink);
            return true;
        }
        else
        {
            Logger.LogInformation("Invite link is not recognised for target group: {TargetGroupId}, inviteLinkName: {InviteLinkName}, inviteLink: {InviteLink}", Butler.Options.TargetGroupId, inviteLink.Name, inviteLink.InviteLink);
            return false;
        }
    }

    private async Task DoHandleChatJoinRequestAsync(User from, long userChatId, CancellationToken cancellationToken)
    {
        Logger.LogInformation("New join request in target group: {TargetGroupId}, userID: {UserId}", Butler.Options.TargetGroupId, from.Id);

        var originalRequest = await UserRepository.FindJoinRequestAsync(from.Id, cancellationToken);

        if (originalRequest == null || !originalRequest.IsWhoisProvided)
        {
            await Butler.TargetGroup.DeclineJoinRequestAsync(from.Id, cancellationToken);
        }
        else
        {
            var withChatId = originalRequest with { UserChatId = userChatId };
            await UserRepository.UpdateJoinRequestAsync(withChatId, cancellationToken);

            if (Butler.Options.WhoisReviewMode == WhoisReviewMode.PreJoin)
            {
                await Butler.AdminGroup.ReportJoinRequestAsync(from, originalRequest.Whois, cancellationToken);
            }
            else
            {
                await Butler.TargetGroup.ApproveJoinRequestAsync(from.Id, cancellationToken);
            }
        }
    }
}
