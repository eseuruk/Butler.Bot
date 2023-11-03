using Butler.Bot.Core.AdminGroup;
using Butler.Bot.Core.UserChat;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot.Types;

namespace Butler.Bot.Core.TargetGroup;

public class ChatMemberAddedHandler : IUpdateHandler
{
    private readonly ButlerOptions options;
    private readonly IUserChatBot userChatBot;
    private readonly ITargetGroupBot targetGroupBot;
    private readonly IAdminGroupBot adminGroupBot;
    private readonly IUserRepository userRepository;

    private readonly ILogger<ChatMemberAddedHandler> logger;

    public ChatMemberAddedHandler(IOptions<ButlerOptions> options, IUserChatBot userChatBot, ITargetGroupBot targetGroupBot, IAdminGroupBot adminGroupBot, IUserRepository userRepository, ILogger<ChatMemberAddedHandler> logger)
    {
        this.options = options.Value;
        this.userChatBot = userChatBot;
        this.targetGroupBot = targetGroupBot;
        this.adminGroupBot = adminGroupBot;
        this.userRepository = userRepository;
        this.logger = logger;
    }

    public async Task<bool> TryHandleUpdateAsync(Update update, CancellationToken cancellationToken)
    {
        // Only handle add member messages
        if (update.Message == null || update.Message.NewChatMembers == null) return false;

        // Only handle messages from target group
        if (update.Message.Chat.Id != options.TargetGroupId) return false;

        await DoHadleChatMembersAdded(update.Message.NewChatMembers, cancellationToken);
        return true;
    }

    private async Task DoHadleChatMembersAdded(User[] newChatMembers, CancellationToken cancellationToken)
    {
        foreach (var newMember in newChatMembers)
        {
            logger.LogInformation("New member event in target group: {TargetGroupId}, userId: {UserId}", options.TargetGroupId, newMember.Id);

            var request = await userRepository.FindJoinRequestAsync(newMember.Id, cancellationToken);

            if (request == null || !request.IsWhoisProvided)
            {
                await targetGroupBot.SayHelloToUnknownNewMemberAsync(newMember, cancellationToken);
            }
            else if (request.IsWhoisMessageWritten)
            {
                await targetGroupBot.SayHelloAgainAsync(newMember, cancellationToken);

                if (request.IsUserChatIdSaved)
                {
                    await userChatBot.TrySayingRequestApprovedAsync(request.UserChatId, cancellationToken);
                }
            }
            else
            {
                var whoisMessage = await targetGroupBot.SayHelloToNewMemberAsync(newMember, request.Whois, cancellationToken);

                var updatedRequst = request with { WhoisMessageId = whoisMessage.MessageId };
                await userRepository.UpdateJoinRequestAsync(updatedRequst, cancellationToken);

                if (request.IsUserChatIdSaved)
                {
                    await userChatBot.TrySayingRequestApprovedAsync(request.UserChatId, cancellationToken);
                }

                if (options.WhoisReviewMode == WhoisReviewMode.PostJoin)
                {
                    await adminGroupBot.ReportUserAddedAsync(newMember, whoisMessage.MessageId, request.Whois, cancellationToken);
                }
            }
        }
    }
}
