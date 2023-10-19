using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Butler.Bot.Core.AdminGroup;

public class AdminGroupBot : GroupBotBase, IAdminGroupBot
{
    private readonly IInlineStateManager inlineStateManager;
    private readonly IAdminGroupMentionStrategy mentionStrategy;

    public AdminGroupBot(ITelegramBotClient apiClient, IOptions<ButlerOptions> options, ILogger<AdminGroupBot> logger, IInlineStateManager inlineStateManager, IAdminGroupMentionStrategy mentionStrategy)
        : base(apiClient, options, logger)
    {
        this.inlineStateManager = inlineStateManager;
        this.mentionStrategy = mentionStrategy;
    }

    public async Task<Message> ReportJoinRequestAsync(User user, string whois, CancellationToken cancellationToken)
    {
        var userMention = mentionStrategy.GetUserMention(user);
        var text = Options.AdminGroupMessages.ReportJoinRequest.SafeFormat(userMention, whois);
        text = inlineStateManager.InjectStateIntoMessageHtml(text, user);

        var markup = new InlineKeyboardMarkup(new[]
            {
                InlineKeyboardButton.WithCallbackData(Options.AdminGroupMessages.ButtonApprove, "prejoin-approve"),
                InlineKeyboardButton.WithCallbackData(Options.AdminGroupMessages.ButtonDecline, "prejoin-decline")
            });

        var message = await ApiClient.SendTextMessageAsync(
            chatId: Options.AdminGroupId,
            text: text,
            parseMode: ParseMode.Html,
            disableWebPagePreview: true,
            replyMarkup: markup,
            cancellationToken: cancellationToken);

        Logger.LogInformation("Reported new join request to admin group: {ChatId}, userId: {UserId} messageId: {MessageId}", Options.AdminGroupId, user.Id, message.MessageId);
        return message;
    }

    public async Task MarkJoinRequestAsApprovedAsync(int messageId, User admin, CancellationToken cancellationToken)
    {
        var adminMention = mentionStrategy.GetAdminMention(admin);
        var buttonText = Options.AdminGroupMessages.MarkJoinRequestAsApproved.SafeFormat(adminMention);

        var markup = new InlineKeyboardMarkup(new[]
            {
                InlineKeyboardButton.WithCallbackData(buttonText, "prejoin-done")
            });

        await ApiClient.EditMessageReplyMarkupAsync(
            chatId: Options.AdminGroupId,
            messageId: messageId,
            replyMarkup: markup,
            cancellationToken: cancellationToken);

        Logger.LogInformation("Join request is marked as approved in admin group: {ChatId} messageId: {MessageId}, adminId: {AdminId}", Options.AdminGroupId, messageId, admin.Id);
    }

    public async Task MarkJoinRequestAsDeclinedAsync(int messageId, User admin, CancellationToken cancellationToken)
    {
        var adminMention = mentionStrategy.GetAdminMention(admin);
        var buttonText = Options.AdminGroupMessages.MarkJoinRequestAsDeclined.SafeFormat(adminMention);

        var markup = new InlineKeyboardMarkup(new[]
            {
                InlineKeyboardButton.WithCallbackData(buttonText, "prejoin-done")
            });

        await ApiClient.EditMessageReplyMarkupAsync(
            chatId: Options.AdminGroupId,
            messageId: messageId,
            replyMarkup: markup,
            cancellationToken: cancellationToken);

        Logger.LogInformation("Join request is marked as declined in admin group: {ChatId} messageId: {MessageId}, adminId: {AdminId}", Options.AdminGroupId, messageId, admin.Id);
    }


    public async Task ReportUserAddedAsync(User user, string whois, CancellationToken cancellationToken)
    {
        var userMention = mentionStrategy.GetUserMention(user);
        var text = Options.AdminGroupMessages.ReportUserAdded.SafeFormat(userMention, whois);
        text = inlineStateManager.InjectStateIntoMessageHtml(text, user);

        var markup = new InlineKeyboardMarkup(new[]
            {
                InlineKeyboardButton.WithCallbackData(Options.AdminGroupMessages.ButtonDelete, "postjoin-delete")
            });

        await ApiClient.SendTextMessageAsync(
            chatId: Options.AdminGroupId,
            text: text,
            parseMode: ParseMode.Html,
            disableWebPagePreview: true,
            replyMarkup: markup,
            cancellationToken: cancellationToken);

        Logger.LogInformation("Reported new group member to admin group: {ChatId}, userId: {UserId}", Options.AdminGroupId, user.Id);
    }

    public async Task AskForUserDeleteConfirmationAsync(int messageId, CancellationToken cancellationToken)
    {
        var markup = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(Options.AdminGroupMessages.ButtonDeleteConfirm, "postjoin-delete-confirm"),
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(Options.AdminGroupMessages.ButtonCancel, "postjoin-delete-cancel"),
                }
            });

        await ApiClient.EditMessageReplyMarkupAsync(
            chatId: Options.AdminGroupId,
            messageId: messageId,
            replyMarkup: markup,
            cancellationToken: cancellationToken);

        Logger.LogInformation("Asked for delete confirmation in admin group: {ChatId} messageId: {MessageId}", Options.AdminGroupId, messageId);
    }

    public async Task CancelUserDeletionAsync(int messageId, CancellationToken cancellationToken)
    {
        var markup = new InlineKeyboardMarkup(new[]
            {
                InlineKeyboardButton.WithCallbackData(Options.AdminGroupMessages.ButtonDelete, "postjoin-delete")
            });

        await ApiClient.EditMessageReplyMarkupAsync(
            chatId: Options.AdminGroupId,
            messageId: messageId,
            replyMarkup: markup,
            cancellationToken: cancellationToken);

        Logger.LogInformation("Cancel user deletion in admin group: {ChatId} messageId: {MessageId}", Options.AdminGroupId, messageId);
    }

    public async Task MarkUserAsDeletedAsync(int messageId, User admin, CancellationToken cancellationToken)
    {
        var adminMention = mentionStrategy.GetAdminMention(admin);
        var buttonText = Options.AdminGroupMessages.MarkUserAsDeleted.SafeFormat(adminMention);

        var markup = new InlineKeyboardMarkup(new[]
            {
                InlineKeyboardButton.WithCallbackData(buttonText, "postjoin-deleted")
            });

        await ApiClient.EditMessageReplyMarkupAsync(
            chatId: Options.AdminGroupId,
            messageId: messageId,
            replyMarkup: markup,
            cancellationToken: cancellationToken);

        Logger.LogInformation("User is marked as deleted in admin group: {ChatId} messageId: {MessageId}, adminId: {AdminId}", Options.AdminGroupId, messageId, admin.Id);
    }
}

