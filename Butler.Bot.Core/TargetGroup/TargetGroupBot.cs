using Microsoft.Extensions.Logging;
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

    public async Task<bool> IsAreadyMemberAsync(long userId, CancellationToken cancellationToken)
    {
        var member = await apiClient.GetChatMemberAsync(options.TargetGroupId, userId, cancellationToken);

        bool result = member != null && member.Status != ChatMemberStatus.Left && member.Status != ChatMemberStatus.Kicked;

        logger.LogInformation("User group membership check. target group: {ChatId} user: {UserId} result: {Result} details: {Details}", options.TargetGroupId, userId, result, member?.Status);

        return result;
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
        var userMention = MentionBuider.GetMention(user);

        var message = await apiClient.SendTextMessageAsync(
            chatId: options.TargetGroupId,
            text: $"Приветствуем {userMention}\r\n#whois\r\n{whois}",
            parseMode: ParseMode.Html,
            cancellationToken: cancellationToken);

        logger.LogInformation("Said hello to the new member in target group: {ChatId}, user: {UserId}, meesageId: {MessageId}", options.TargetGroupId, user.Id, message.MessageId);
        return message;
    }

    public async Task SayHelloToUnknownNewMemberAsync(User user, CancellationToken cancellationToken)
    {
        var userMention = MentionBuider.GetMention(user);

        await apiClient.SendTextMessageAsync(
            chatId: options.TargetGroupId,
            text: $"Приветствуем {userMention}",
            parseMode: ParseMode.Html,
            cancellationToken: cancellationToken);

        logger.LogInformation("Said hello to unknown new member in target group: {ChatId}, userId: {UserId}", options.TargetGroupId, user.Id);
    }

    public async Task SayHelloAgainAsync(User user, CancellationToken cancellationToken)
    {
        var userMention = MentionBuider.GetMention(user);

        await apiClient.SendTextMessageAsync(
            chatId: options.TargetGroupId,
            text: $"С возвращением {userMention}",
            parseMode: ParseMode.Html,
            cancellationToken: cancellationToken);

        logger.LogInformation("Said hello again in target group: {ChatId}, userId: {UserId}", options.TargetGroupId, user.Id);
    }

    public async Task DeleteUserAsync(long userId, CancellationToken cancellationToken)
    {
        await apiClient.BanChatMemberAsync(options.TargetGroupId, userId);
        await apiClient.UnbanChatMemberAsync(options.TargetGroupId, userId);

        logger.LogInformation("User deleted from target group: {ChatId} user: {UserId}", options.TargetGroupId, userId);
    }

    public async Task DeleteMessageAsync(int messageId, CancellationToken cancellationToken)
    {
        await apiClient.DeleteMessageAsync(options.TargetGroupId, messageId);
        logger.LogInformation("Message is deleted from target group: {ChatId} messageId: {MessageId}", options.TargetGroupId, messageId);
    }
}

