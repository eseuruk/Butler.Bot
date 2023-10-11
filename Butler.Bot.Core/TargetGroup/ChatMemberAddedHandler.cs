using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;

namespace Butler.Bot.Core.TargetGroup;

public class ChatMemberAddedHandler : UpdateHandlerBase
{
    public ChatMemberAddedHandler(IButlerBot butler, IUserRepository userRepository, ILogger<ChatMemberAddedHandler> logger)
        : base(butler, userRepository, logger)
    {}

    public override async Task<bool> TryHandleUpdateAsync(Update update, CancellationToken cancellationToken)
    {
        // Only handle add member messages
        if (update.Message == null || update.Message.NewChatMembers == null) return false;

        // Only handle messages from target group
        if (update.Message.Chat.Id != Butler.Options.TargetGroupId) return false;

        await DoHadleChatMembersAdded(update.Message.NewChatMembers, cancellationToken);
        return true;
    }

    private async Task DoHadleChatMembersAdded(User[] newChatMembers, CancellationToken cancellationToken)
    {
        foreach (var newMember in newChatMembers)
        {
            Logger.LogInformation("New member event in target group: {TargetGroupId}, userId: {UserId}", Butler.Options.TargetGroupId, newMember.Id);

            var request = await UserRepository.FindJoinRequestAsync(newMember.Id, cancellationToken);

            if (request == null || !request.IsWhoisProvided)
            {
                await Butler.TargetGroup.SayHelloToUnknownNewMemberAsync(newMember, cancellationToken);
            }
            else if (request.IsWhoisMessageWritten)
            {
                await Butler.TargetGroup.SayHelloAgainAsync(newMember, cancellationToken);
            }
            else
            {
                var whoisMessage = await Butler.TargetGroup.SayHelloToNewMemberAsync(newMember, request.Whois, cancellationToken);

                var updatedRequst = request with { WhoisMessageId = whoisMessage.MessageId };
                await UserRepository.UpdateJoinRequestAsync(updatedRequst, cancellationToken);

                if (request.IsUserChatIdSaved)
                {
                    await Butler.UserChat.TrySayingRequestApprovedAsync(request.UserChatId, cancellationToken);
                }

                if (Butler.Options.WhoisReviewMode == WhoisReviewMode.PostJoin)
                {
                    await Butler.AdminGroup.ReportUserAddedAsync(newMember, request.Whois, cancellationToken);
                }
            }
        }
    }
}
