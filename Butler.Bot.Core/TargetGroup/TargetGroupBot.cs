using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Butler.Bot.Core.TargetGroup;

public class TargetGroupBot : GroupBotBase, ITargetGroupBot
{
    public TargetGroupBot(ITelegramBotClient apiClient, IOptions<ButlerOptions> options, ILogger<TargetGroupBot> logger)
        : base(apiClient, options, logger)
    {
    }

    public async Task<ChatMemberStatus?> GetMemberStatusAsync(long userId, CancellationToken cancellationToken)
    {
        var member = await ApiClient.GetChatMemberAsync(Options.TargetGroupId, userId, cancellationToken);
        var memberStatus = member?.Status;

        Logger.LogInformation("User group membership check. target group: {ChatId} user: {UserId} status: {Status}", Options.TargetGroupId, userId, memberStatus);
        return memberStatus;
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
        var userMention = GetUserMention(user);

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
        var userMention = GetUserMention(user);

        await ApiClient.SendTextMessageAsync(
            chatId: Options.TargetGroupId,
            text: Options.TargetGroupMessages.SayHelloToUnknownNewMember.SafeFormat(userMention),
            parseMode: ParseMode.Html,
            cancellationToken: cancellationToken);

        Logger.LogInformation("Said hello to unknown new member in target group: {ChatId}, userId: {UserId}", Options.TargetGroupId, user.Id);
    }

    public async Task SayHelloAgainAsync(User user, CancellationToken cancellationToken)
    {
        var userMention = GetUserMention(user);

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

    public async Task DeleteMessageAsync(int messageId, CancellationToken cancellationToken)
    {
        await ApiClient.DeleteMessageAsync(Options.TargetGroupId, messageId);
        Logger.LogInformation("Message is deleted from target group: {ChatId} messageId: {MessageId}", Options.TargetGroupId, messageId);
    }

    public async Task SayLeavingToChangeWhoisAsync(User user, CancellationToken cancellationToken)
    {
        var userMention = GetUserMention(user);

        await ApiClient.SendTextMessageAsync(
            chatId: Options.TargetGroupId,
            text: Options.TargetGroupMessages.SayLeavingToChangeWhois.SafeFormat(userMention),
            parseMode: ParseMode.Html,
            cancellationToken: cancellationToken);

        Logger.LogInformation("Said user is leaving target group to change whois. group: {ChatId}, userId: {UserId}", Options.TargetGroupId, user.Id);
    }

    public static string GetUserMention(User user)
    {
        var displayName = user.FirstName;
        if (!string.IsNullOrEmpty(user.LastName))
        {
            displayName += " " + user.LastName;
        }

        return $"<a href='tg://user?id={user.Id}'>{displayName}</a>";
    }
}

