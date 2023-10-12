using Butler.Bot.Core.AdminGroup;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot.Types;

namespace Butler.Bot.Core.TargetGroup;

public class JoinRequestHandler : IUpdateHandler
{
    private readonly ButlerOptions options;
    private readonly ITargetGroupBot targetGroupBot;
    private readonly IAdminGroupBot adminGroupBot;
    private readonly IUserRepository userRepository;

    private readonly ILogger<JoinRequestHandler> logger;

    public JoinRequestHandler(IOptions<ButlerOptions> options, ITargetGroupBot targetGroupBot, IAdminGroupBot adminGroupBot, IUserRepository userRepository, ILogger<JoinRequestHandler> logger)
    {
        this.options = options.Value;
        this.targetGroupBot = targetGroupBot;
        this.adminGroupBot = adminGroupBot;
        this.userRepository = userRepository;
        this.logger = logger;
    }

    public async Task<bool> TryHandleUpdateAsync(Update update, CancellationToken cancellationToken)
    {
        // Only handle chat join requests
        if (update.ChatJoinRequest == null) return false;

        // Only handle requests from target group
        if (update.ChatJoinRequest.Chat.Id != options.TargetGroupId) return false;

        // Only handle requests with invite link
        if (update.ChatJoinRequest.InviteLink == null) return false;

        // Only handle requests with our dedicated invite link
        if (!CheckInviteLinkOwnership(update.ChatJoinRequest.InviteLink)) return false;

        await DoHandleChatJoinRequestAsync(update.ChatJoinRequest.From, update.ChatJoinRequest.UserChatId, cancellationToken);
        return true;
    }

    private bool CheckInviteLinkOwnership(ChatInviteLink inviteLink)
    {
        if (string.IsNullOrEmpty(options.InvitationLinkName))
        {
            logger.LogInformation("Invite link ownership check is off for target group: {TargetGroupId}, inviteLink: {InviteLink}", options.TargetGroupId, inviteLink.InviteLink);
            return true;
        }
        else if (options.InvitationLinkName.Equals(inviteLink.Name, StringComparison.OrdinalIgnoreCase))
        {
            logger.LogInformation("Invite link is recognised for target group: {TargetGroupId}, inviteLinkName: {InviteLinkName}, inviteLink: {InviteLink}", options.TargetGroupId, inviteLink.Name, inviteLink.InviteLink);
            return true;
        }
        else
        {
            logger.LogInformation("Invite link is not recognised for target group: {TargetGroupId}, inviteLinkName: {InviteLinkName}, inviteLink: {InviteLink}", options.TargetGroupId, inviteLink.Name, inviteLink.InviteLink);
            return false;
        }
    }

    private async Task DoHandleChatJoinRequestAsync(User from, long userChatId, CancellationToken cancellationToken)
    {
        logger.LogInformation("New join request in target group: {TargetGroupId}, userID: {UserId}", options.TargetGroupId, from.Id);

        var originalRequest = await userRepository.FindJoinRequestAsync(from.Id, cancellationToken);

        if (originalRequest == null || !originalRequest.IsWhoisProvided)
        {
            await targetGroupBot.DeclineJoinRequestAsync(from.Id, cancellationToken);
        }
        else
        {
            var withChatId = originalRequest with { UserChatId = userChatId };
            await userRepository.UpdateJoinRequestAsync(withChatId, cancellationToken);

            if (options.WhoisReviewMode == WhoisReviewMode.PreJoin)
            {
                await adminGroupBot.ReportJoinRequestAsync(from, originalRequest.Whois, cancellationToken);
            }
            else
            {
                await targetGroupBot.ApproveJoinRequestAsync(from.Id, cancellationToken);
            }
        }
    }
}
