using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Butler.Bot.Core.AdminGroup;

public class AdminGroupBot
{
    private readonly ITelegramBotClient apiClient;
    private readonly ButlerOptions options;

    private readonly ILogger<AdminGroupBot> logger;

    public AdminGroupBot(ITelegramBotClient apiClient, IOptions<ButlerOptions> options, ILogger<AdminGroupBot> logger)
    {
        this.apiClient = apiClient;
        this.options = options.Value;
        this.logger = logger;
    }

    public async Task<Message> ReportJoinRequestAsync(User user, string whois, CancellationToken cancellationToken)
    {
        var userMention = MentionBuider.GetMention(user);

        var markup = new InlineKeyboardMarkup(new[]
            {
                InlineKeyboardButton.WithCallbackData(options.AdminGroupMessages.ButtonApprove, ReviewQueryData.ToString("prejoin-approve", user.Id)),
                InlineKeyboardButton.WithCallbackData(options.AdminGroupMessages.ButtonDecline, ReviewQueryData.ToString("prejoin-decline", user.Id))
            });

        var message = await apiClient.SendTextMessageAsync(
            chatId: options.AdminGroupId,
            text: options.AdminGroupMessages.ReportJoinRequest.SafeFormat(userMention, whois),
            parseMode: ParseMode.Html,
            replyMarkup: markup,
            cancellationToken: cancellationToken);

        logger.LogInformation("Reported new join request to admin group: {ChatId}, userId: {UserId} messageId: {MessageId}", options.AdminGroupId, user.Id, message.MessageId);
        return message;
    }

    public async Task ReportUserAddedAsync(User user, string whois, CancellationToken cancellationToken)
    {
        var userMention = MentionBuider.GetMention(user);

        var markup = new InlineKeyboardMarkup(new[]
            {
                InlineKeyboardButton.WithCallbackData(options.AdminGroupMessages.ButtonDelete, ReviewQueryData.ToString("postjoin-delete", user.Id))
            });

        await apiClient.SendTextMessageAsync(
            chatId: options.AdminGroupId,
            text: options.AdminGroupMessages.ReportUserAdded.SafeFormat(userMention, whois),
            parseMode: ParseMode.Html,
            replyMarkup: markup,
            cancellationToken: cancellationToken);

        logger.LogInformation("Reported new group member to admin group: {ChatId}, userId: {UserId}", options.AdminGroupId, user.Id);
    }

    public async Task MarkJoinRequestAsApprovedAsync(int messageId, User admin, CancellationToken cancellationToken)
    {
        var adminMention = MentionBuider.GetMention(admin);

        await apiClient.SendTextMessageAsync(
            chatId: options.AdminGroupId,
            replyToMessageId: messageId,
            text: options.AdminGroupMessages.MarkJoinRequestAsApproved.SafeFormat(adminMention),
            parseMode: ParseMode.Html,
            cancellationToken: cancellationToken);

        logger.LogInformation("Join request is marked as approved in admin group: {ChatId} messageId: {MessageId}, adminId: {AdminId}", options.AdminGroupId, messageId, admin);
    }

    public async Task MarkJoinRequestAsDeclinedAsync(int messageId, User admin, CancellationToken cancellationToken)
    {
        var adminMention = MentionBuider.GetMention(admin);

        await apiClient.SendTextMessageAsync(
            chatId: options.AdminGroupId,
            replyToMessageId: messageId,
            text: options.AdminGroupMessages.MarkJoinRequestAsDeclined.SafeFormat(adminMention),
            parseMode: ParseMode.Html,
            cancellationToken: cancellationToken);

        logger.LogInformation("Join request is marked as declined in admin group: {ChatId} messageId: {MessageId}, adminId: {AdminId}", options.AdminGroupId, messageId, admin);
    }

    public async Task MarkUserAsDeletedAsync(int messageId, User admin, CancellationToken cancellationToken)
    {
        var adminMention = MentionBuider.GetMention(admin);

        await apiClient.SendTextMessageAsync(
            chatId: options.AdminGroupId,
            replyToMessageId: messageId,
            text: options.AdminGroupMessages.MarkUserAsDeleted.SafeFormat(adminMention),
            parseMode: ParseMode.Html,
            cancellationToken: cancellationToken);

        logger.LogInformation("User is marked as deleted in admin group: {ChatId} messageId: {MessageId}, adminId: {AdminId}", options.AdminGroupId, messageId, admin);
    }
}

