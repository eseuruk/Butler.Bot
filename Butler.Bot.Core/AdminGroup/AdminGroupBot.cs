using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using System.Text;

namespace Butler.Bot.Core.AdminGroup;

public class AdminGroupBot
{
    private readonly ITelegramBotClient apiClient;
    private readonly InlineStateManager inlineStateManager;
    private readonly ButlerOptions options;

    private readonly ILogger<AdminGroupBot> logger;

    public AdminGroupBot(ITelegramBotClient apiClient, InlineStateManager inlineStateManager, IOptions<ButlerOptions> options, ILogger<AdminGroupBot> logger)
    {
        this.apiClient = apiClient;
        this.inlineStateManager = inlineStateManager;
        this.options = options.Value;
        this.logger = logger;
    }

    public async Task<Message> ReportJoinRequestAsync(User user, string whois, CancellationToken cancellationToken)
    {
        var userMention = GetUserMention(user);
        var text = options.AdminGroupMessages.ReportJoinRequest.SafeFormat(userMention, whois);
        text = inlineStateManager.InjectStateIntoMessageHtml(text, user);

        var markup = new InlineKeyboardMarkup(new[]
            {
                InlineKeyboardButton.WithCallbackData(options.AdminGroupMessages.ButtonApprove, "prejoin-approve"),
                InlineKeyboardButton.WithCallbackData(options.AdminGroupMessages.ButtonDecline, "prejoin-decline")
            });

        var message = await apiClient.SendTextMessageAsync(
            chatId: options.AdminGroupId,
            text: text,
            parseMode: ParseMode.Html,
            disableWebPagePreview: true,
            replyMarkup: markup,
            cancellationToken: cancellationToken);

        logger.LogInformation("Reported new join request to admin group: {ChatId}, userId: {UserId} messageId: {MessageId}", options.AdminGroupId, user.Id, message.MessageId);
        return message;
    }

    public async Task ReportUserAddedAsync(User user, string whois, CancellationToken cancellationToken)
    {
        var userMention = GetUserMention(user);
        var text = options.AdminGroupMessages.ReportUserAdded.SafeFormat(userMention, whois);
        text = inlineStateManager.InjectStateIntoMessageHtml(text, user);

        var markup = new InlineKeyboardMarkup(new[]
            {
                InlineKeyboardButton.WithCallbackData(options.AdminGroupMessages.ButtonDelete, "postjoin-delete")
            });

        await apiClient.SendTextMessageAsync(
            chatId: options.AdminGroupId,
            text: text,
            parseMode: ParseMode.Html,
            disableWebPagePreview: true,
            replyMarkup: markup,
            cancellationToken: cancellationToken);

        logger.LogInformation("Reported new group member to admin group: {ChatId}, userId: {UserId}", options.AdminGroupId, user.Id);
    }

    public async Task MarkJoinRequestAsApprovedAsync(int messageId, User admin, CancellationToken cancellationToken)
    {
        var adminMention = GetAdminMention(admin);

        await apiClient.SendTextMessageAsync(
            chatId: options.AdminGroupId,
            replyToMessageId: messageId,
            text: options.AdminGroupMessages.MarkJoinRequestAsApproved.SafeFormat(adminMention),
            parseMode: ParseMode.Html,
            cancellationToken: cancellationToken);

        logger.LogInformation("Join request is marked as approved in admin group: {ChatId} messageId: {MessageId}, adminId: {AdminId}", options.AdminGroupId, messageId, admin.Id);
    }

    public async Task MarkJoinRequestAsDeclinedAsync(int messageId, User admin, CancellationToken cancellationToken)
    {
        var adminMention = GetAdminMention(admin);

        await apiClient.SendTextMessageAsync(
            chatId: options.AdminGroupId,
            replyToMessageId: messageId,
            text: options.AdminGroupMessages.MarkJoinRequestAsDeclined.SafeFormat(adminMention),
            parseMode: ParseMode.Html,
            cancellationToken: cancellationToken);

        logger.LogInformation("Join request is marked as declined in admin group: {ChatId} messageId: {MessageId}, adminId: {AdminId}", options.AdminGroupId, messageId, admin.Id);
    }

    public async Task MarkUserAsDeletedAsync(int messageId, User admin, CancellationToken cancellationToken)
    {
        var adminMention = GetAdminMention(admin);

        await apiClient.SendTextMessageAsync(
            chatId: options.AdminGroupId,
            replyToMessageId: messageId,
            text: options.AdminGroupMessages.MarkUserAsDeleted.SafeFormat(adminMention),
            parseMode: ParseMode.Html,
            cancellationToken: cancellationToken);

        logger.LogInformation("User is marked as deleted in admin group: {ChatId} messageId: {MessageId}, adminId: {AdminId}", options.AdminGroupId, messageId, admin.Id);
    }

    private static string GetAdminMention(User user)
    {
        if (!string.IsNullOrEmpty(user.Username))
        {
            return "@" + user.Username;
        }

        var displayName = user.FirstName;
        if (!string.IsNullOrEmpty(user.LastName))
        {
            displayName += " " + user.LastName;
        }

        return $"<a href='tg://user?id={user.Id}'>{displayName}</a>";
    }


    private static string GetUserMention(User user)
    {
        var mention = new StringBuilder();

        mention.Append(user.FirstName);

        if (!string.IsNullOrEmpty(user.LastName))
        {
            mention.Append(' ');
            mention.Append(user.LastName);
        }

        if (!string.IsNullOrEmpty(user.Username))
        {
            mention.Append(" @");
            mention.Append(user.Username);
        }

        return mention.ToString();
    }
}

