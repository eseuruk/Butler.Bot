using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Butler.Bot.Core.TargetGroup;

public class TargetGroupBot : GroupBotBase, ITargetGroupBot
{
    private readonly ITargetGroupMentionStrategy mentionStrategy;

    public TargetGroupBot(ITelegramBotClient apiClient, IOptions<ButlerOptions> options, ILogger<TargetGroupBot> logger, ITargetGroupMentionStrategy mentionStrategy)
        : base(apiClient, options, logger)
    {
        this.mentionStrategy = mentionStrategy;
    }

    public async Task<ChatMember> GetChatMemberAsync(long userId, CancellationToken cancellationToken)
    {
        var member = await ApiClient.GetChatMemberAsync(Options.TargetGroupId, userId, cancellationToken);

        Logger.LogInformation("Get group membership. target group: {ChatId} user: {UserId} status: {Status}, retUser: {RetUserId}", Options.TargetGroupId, userId, member.Status, member.User.Id);
        return member;
    }

    public async Task DeclineJoinRequestAsync(long userId, CancellationToken cancellationToken)
    {
        await ApiClient.DeclineChatJoinRequest(Options.TargetGroupId, userId, cancellationToken);

        Logger.LogInformation("Join request declined to target group: {ChatId} user: {UserId}", Options.TargetGroupId, userId);
    }

    public async Task ApproveJoinRequestAsync(long userId, CancellationToken cancellationToken)
    {
        await ApiClient.ApproveChatJoinRequest(Options.TargetGroupId, userId, cancellationToken);

        Logger.LogInformation("Join request approved to target group: {ChatId} user: {UserId}", Options.TargetGroupId, userId);
    }

    public async Task<Message> SayHelloToNewMemberAsync(User user, string whois, CancellationToken cancellationToken)
    {
        var userMention = mentionStrategy.GetUserMention(user);

        var message = await ApiClient.SendTextMessageAsync(
            chatId: Options.TargetGroupId,
            text: Options.TargetGroupMessages.SayHelloToNewMember.SafeFormat(userMention, whois),
            parseMode: ParseMode.Html,
            disableWebPagePreview: true,
            cancellationToken: cancellationToken);

        Logger.LogInformation("Said hello to the new member in target group: {ChatId}, user: {UserId}, meesageId: {MessageId}", Options.TargetGroupId, user.Id, message.MessageId);
        return message;
    }

    public async Task SayHelloToUnknownNewMemberAsync(User user, CancellationToken cancellationToken)
    {
        var userMention = mentionStrategy.GetUserMention(user);

        await ApiClient.SendTextMessageAsync(
            chatId: Options.TargetGroupId,
            text: Options.TargetGroupMessages.SayHelloToUnknownNewMember.SafeFormat(userMention),
            parseMode: ParseMode.Html,
            cancellationToken: cancellationToken);

        Logger.LogInformation("Said hello to unknown new member in target group: {ChatId}, userId: {UserId}", Options.TargetGroupId, user.Id);
    }

    public async Task SayHelloAgainAsync(User user, CancellationToken cancellationToken)
    {
        var userMention = mentionStrategy.GetUserMention(user);

        await ApiClient.SendTextMessageAsync(
            chatId: Options.TargetGroupId,
            text: Options.TargetGroupMessages.SayHelloAgain.SafeFormat(userMention),
            parseMode: ParseMode.Html,
            cancellationToken: cancellationToken);

        Logger.LogInformation("Said hello again in target group: {ChatId}, userId: {UserId}", Options.TargetGroupId, user.Id);
    }

    public async Task DeleteUserAsync(long userId, CancellationToken cancellationToken)
    {
        // Per API documentation this call is enough to remove user from the chat but not block
        await ApiClient.UnbanChatMemberAsync(Options.TargetGroupId, userId, null, cancellationToken);

        // Get the user status after removing to log it for investigations
        var user = await ApiClient.GetChatMemberAsync(Options.TargetGroupId, userId, cancellationToken);

        Logger.LogInformation("User deleted from target group: {ChatId} user: {UserId} currentStatus: {userStatus}", Options.TargetGroupId, userId, user.Status);
    }

    public async Task TryDeleteMessageAsync(int messageId, CancellationToken cancellationToken)
    {
        try
        {
            await ApiClient.DeleteMessageAsync(Options.TargetGroupId, messageId);
            Logger.LogInformation("Message is deleted from target group: {ChatId} messageId: {MessageId}", Options.TargetGroupId, messageId);
        }
        catch (ApiRequestException ex)
        {
            // Message might be deleted already
            Logger.LogWarning("Cannot delete message in target group: {ChatId}, messageId: {MessageId}, errorCode: {ErrorCode}, errorMessage: {ErrorMessage}", Options.TargetGroupId, messageId, ex.ErrorCode, ex.Message);
        }
    }

    public async Task SayLeavingToChangeWhoisAsync(User user, CancellationToken cancellationToken)
    {
        var userMention = mentionStrategy.GetUserMention(user);

        await ApiClient.SendTextMessageAsync(
            chatId: Options.TargetGroupId,
            text: Options.TargetGroupMessages.SayLeavingToChangeWhois.SafeFormat(userMention),
            parseMode: ParseMode.Html,
            cancellationToken: cancellationToken);

        Logger.LogInformation("Said user is leaving target group to change whois. group: {ChatId}, userId: {UserId}", Options.TargetGroupId, user.Id);
    }

    public async Task SayLeavingAsync(User user, CancellationToken cancellationToken)
    {
        var userMention = mentionStrategy.GetUserMention(user);

        await ApiClient.SendTextMessageAsync(
            chatId: Options.TargetGroupId,
            text: Options.TargetGroupMessages.SayLeaving.SafeFormat(userMention),
            parseMode: ParseMode.Html,
            cancellationToken: cancellationToken);

        Logger.LogInformation("Said user is leaving target group. group: {ChatId}, userId: {UserId}", Options.TargetGroupId, user.Id);
    }

}

