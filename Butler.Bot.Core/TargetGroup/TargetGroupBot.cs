﻿using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Butler.Bot.Core.TargetGroup;

public class TargetGroupBot
{
    private readonly ITelegramBotClient apiClient;
    private readonly ButlerOptions options;

    private readonly ILogger<TargetGroupBot> logger;

    public TargetGroupBot(ITelegramBotClient apiClient, IOptions<ButlerOptions> options, ILogger<TargetGroupBot> logger)
    {
        this.apiClient = apiClient;
        this.options = options.Value;
        this.logger = logger;
    }

    public async Task<ChatMemberStatus?> GetMemberStatusAsync(long userId, CancellationToken cancellationToken)
    {
        var member = await apiClient.GetChatMemberAsync(options.TargetGroupId, userId, cancellationToken);
        var memberStatus = member?.Status;

        logger.LogInformation("User group membership check. target group: {ChatId} user: {UserId} status: {Status}", options.TargetGroupId, userId, memberStatus);
        return memberStatus;
    }

    public async Task DeclineJoinRequestAsync(long userId, CancellationToken cancellationToken)
    {
        await apiClient.DeclineChatJoinRequest(options.TargetGroupId, userId, cancellationToken);

        logger.LogInformation("Join request declined to target group: {ChatId} user: {UserId}", options.TargetGroupId, userId);
    }

    public async Task ApproveJoinRequestAsync(long userId, CancellationToken cancellationToken)
    {
        await apiClient.ApproveChatJoinRequest(options.TargetGroupId, userId, cancellationToken);

        logger.LogInformation("Join request approved to target group: {ChatId} user: {UserId}", options.TargetGroupId, userId);
    }

    public async Task<Message> SayHelloToNewMemberAsync(User user, string whois, CancellationToken cancellationToken)
    {
        var userMention = GetUserMention(user);

        var message = await apiClient.SendTextMessageAsync(
            chatId: options.TargetGroupId,
            text: options.TargetGroupMessages.SayHelloToNewMember.SafeFormat(userMention, whois),
            parseMode: ParseMode.Html,
            disableWebPagePreview: true,
            cancellationToken: cancellationToken);

        logger.LogInformation("Said hello to the new member in target group: {ChatId}, user: {UserId}, meesageId: {MessageId}", options.TargetGroupId, user.Id, message.MessageId);
        return message;
    }

    public async Task SayHelloToUnknownNewMemberAsync(User user, CancellationToken cancellationToken)
    {
        var userMention = GetUserMention(user);

        await apiClient.SendTextMessageAsync(
            chatId: options.TargetGroupId,
            text: options.TargetGroupMessages.SayHelloToUnknownNewMember.SafeFormat(userMention),
            parseMode: ParseMode.Html,
            cancellationToken: cancellationToken);

        logger.LogInformation("Said hello to unknown new member in target group: {ChatId}, userId: {UserId}", options.TargetGroupId, user.Id);
    }

    public async Task SayHelloAgainAsync(User user, CancellationToken cancellationToken)
    {
        var userMention = GetUserMention(user);

        await apiClient.SendTextMessageAsync(
            chatId: options.TargetGroupId,
            text: options.TargetGroupMessages.SayHelloAgain.SafeFormat(userMention),
            parseMode: ParseMode.Html,
            cancellationToken: cancellationToken);

        logger.LogInformation("Said hello again in target group: {ChatId}, userId: {UserId}", options.TargetGroupId, user.Id);
    }

    public async Task DeleteUserAsync(long userId, CancellationToken cancellationToken)
    {
        // Per API documentation this call is enough to remove user from the chat but not block
        await apiClient.UnbanChatMemberAsync(options.TargetGroupId, userId, null, cancellationToken);

        // Get the user status after removing to log it for investigations
        var user = await apiClient.GetChatMemberAsync(options.TargetGroupId, userId, cancellationToken);

        logger.LogInformation("User deleted from target group: {ChatId} user: {UserId} currentStatus: {userStatus}", options.TargetGroupId, userId, user.Status);
    }

    public async Task DeleteMessageAsync(int messageId, CancellationToken cancellationToken)
    {
        await apiClient.DeleteMessageAsync(options.TargetGroupId, messageId);
        logger.LogInformation("Message is deleted from target group: {ChatId} messageId: {MessageId}", options.TargetGroupId, messageId);
    }
        
    public async Task SayLeavingToChangeWhoisAsync(User user, CancellationToken cancellationToken)
    {
        var userMention = GetUserMention(user);

        await apiClient.SendTextMessageAsync(
            chatId: options.TargetGroupId,
            text: options.TargetGroupMessages.SayLeavingToChangeWhois.SafeFormat(userMention),
            parseMode: ParseMode.Html,
            cancellationToken: cancellationToken);

        logger.LogInformation("Said user is leaving target group to change whois. group: {ChatId}, userId: {UserId}", options.TargetGroupId, user.Id);
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

