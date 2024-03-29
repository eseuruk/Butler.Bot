﻿namespace Butler.Bot.Core.UserChat;

public class UserChatBot : GroupBotBase, IUserChatBot
{
    public UserChatBot(ITelegramBotClient apiClient, IOptions<ButlerOptions> options, ILogger<UserChatBot> logger)
        : base(apiClient, options, logger)
    {
    }

    public async Task ShowBotVersionAsync(long userChatId, CancellationToken cancellationToken)
    {
        var bot = await ApiClient.GetMeAsync(cancellationToken);
        var botName = bot.FirstName;
        var botVersion = ButlerVersion.GetCurrent();

        await ApiClient.SendTextMessageAsync(
            chatId: userChatId,
            text: Options.UserChatMessages.ShowBotVersion.SafeFormat(botName, botVersion, Options.SiteUrl),
            parseMode: ParseMode.Html,
            disableWebPagePreview: true,
            cancellationToken: cancellationToken);

        Logger.LogInformation("Showed bot version in private chat: {UserChatId}, botName: {BotName}, botVersion: {BotVersion}, siteUrl: {SiteUrl}", userChatId, botName, botVersion, Options.SiteUrl);
    }

    public async Task SayHelloAsync(long userChatId, CancellationToken cancellationToken)
    {
        var markup = new InlineKeyboardMarkup(
                InlineKeyboardButton.WithCallbackData(Options.UserChatMessages.ButtonApply, "register"));

        await ApiClient.SendTextMessageAsync(
            chatId: userChatId,
            text: Options.UserChatMessages.SayHello.SafeFormat(Options.TargetGroupDisplayName),
            parseMode: ParseMode.Html,
            disableWebPagePreview: true,
            replyMarkup: markup,
            cancellationToken: cancellationToken);

        Logger.LogInformation("Said hello in private chat: {UserChatId}", userChatId);
    }

    public async Task SayConfusedAsync(long userChatId, CancellationToken cancellationToken)
    {
        var markup = new InlineKeyboardMarkup(
            InlineKeyboardButton.WithCallbackData(Options.UserChatMessages.ButtonContinue, "register"));

        await ApiClient.SendTextMessageAsync(
            chatId: userChatId,
            text: Options.UserChatMessages.SayConfused.SafeFormat(Options.TargetGroupDisplayName),
            parseMode: ParseMode.Html,
            disableWebPagePreview: true,
            replyMarkup: markup,
            cancellationToken: cancellationToken);

        Logger.LogInformation("Said confused in private chat: {UserChatId}", userChatId);
    }

    public async Task AskForWhoisAsync(long userChatId, CancellationToken cancellationToken)
    {
        await ApiClient.SendTextMessageAsync(
            chatId: userChatId,
            text: Options.UserChatMessages.AskForWhois.SafeFormat(Options.MinWoisLength),
            parseMode: ParseMode.Html,
            disableWebPagePreview: true,
            cancellationToken: cancellationToken);

        Logger.LogInformation("Asked to provide whois in private chat: {UserChatId}", userChatId);
    }

    public async Task WarnWhoisValidationFailedAsync(long userChatId, string validationError, CancellationToken cancellationToken)
    {
        await ApiClient.SendTextMessageAsync(
            chatId: userChatId,
            text: validationError,
            parseMode: ParseMode.Html,
            disableWebPagePreview: true,
            cancellationToken: cancellationToken);

        Logger.LogInformation("Said whois is not valid in private chat: {UserChatId}", userChatId);
    }

    public async Task SayWhoisOkAndAskToRequestAccessAsync(long userChatId, CancellationToken cancellationToken)
    {
        var markup = new InlineKeyboardMarkup(
                InlineKeyboardButton.WithUrl(Options.TargetGroupDisplayName, Options.InvitationLink));

        await ApiClient.SendTextMessageAsync(
            chatId: userChatId,
            text: Options.UserChatMessages.SayWhoisOkAndAskToRequestAccess,
            parseMode: ParseMode.Html,
            disableWebPagePreview: true,
            replyMarkup: markup,
            cancellationToken: cancellationToken);

        Logger.LogInformation("Said whois is ok in private chat and ask to join target group. private chat: {UserChatId}", userChatId);
    }

    public async Task SayUsedToBeMemberAsync(long userChatId, CancellationToken cancellationToken)
    {
        var markup = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithUrl(Options.UserChatMessages.ButtonRequestToJoin, Options.InvitationLink)
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(Options.UserChatMessages.ButtonDeleteWhois, "register-delete")
                }
            });

        await ApiClient.SendTextMessageAsync(
            chatId: userChatId,
            text: Options.UserChatMessages.SayUsedToBeMember,
            parseMode: ParseMode.Html,
            disableWebPagePreview: true,
            replyMarkup: markup,
            cancellationToken: cancellationToken);

        Logger.LogInformation("Said used to be a member in private chat: {UserChatId}", userChatId);
    }

    public async Task SayAlreadyMemberAsync(long userChatId, CancellationToken cancellationToken)
    {
        var markup = new InlineKeyboardMarkup(
                InlineKeyboardButton.WithUrl(Options.TargetGroupDisplayName, Options.InvitationLink));

        await ApiClient.SendTextMessageAsync(
            chatId: userChatId,
            text: Options.UserChatMessages.SayAlreadyMember,
            parseMode: ParseMode.Html,
            disableWebPagePreview: true,
            replyMarkup: markup,
            cancellationToken: cancellationToken);

        Logger.LogInformation("Said already a member in private chat: {UserChatId}", userChatId);
    }

    public async Task SayBlockedAsync(long userChatId, CancellationToken cancellationToken)
    {
        await ApiClient.SendTextMessageAsync(
            chatId: userChatId,
            text: Options.UserChatMessages.SayBlockedMember,
            parseMode: ParseMode.Html,
            disableWebPagePreview: true,
            cancellationToken: cancellationToken);

        Logger.LogInformation("Said blocked in private chat: {UserChatId}", userChatId);
    }

    public async Task<int> ShowLeaveRequestAsync(long userChatId, CancellationToken cancellationToken)
    {
        var markup = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(Options.UserChatMessages.ButtonLeave, "leave-confirm")
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(Options.UserChatMessages.ButtonCancel, "leave-cancel")
                }
            });

        var message = await ApiClient.SendTextMessageAsync(
            chatId: userChatId,
            text: Options.UserChatMessages.AskForForLeavingConfirmation.SafeFormat(Options.TargetGroupDisplayName),
            parseMode: ParseMode.Html,
            disableWebPagePreview: true,
            replyMarkup: markup,
            cancellationToken: cancellationToken);

        Logger.LogInformation("Showed leave request in private chat: {UserChatId}, MessageId: {MessageId}", userChatId, message.MessageId);
        return message.MessageId;
    }

    public async Task SayLeaveRequestCanceledAsync(long userChatId, int requestMessageId, CancellationToken cancellationToken)
    {
        await ApiClient.EditMessageTextAsync(
            chatId: userChatId,
            messageId: requestMessageId,
            text: Options.UserChatMessages.SayLeavingRequestCanceled.SafeFormat(Options.TargetGroupDisplayName),
            parseMode: ParseMode.Html,
            disableWebPagePreview: true,
            cancellationToken: cancellationToken);

        Logger.LogInformation("Said leave request canceled in private chat: {UserChatId}, requestMessageId: {RequestMessageId}", userChatId, requestMessageId);
    }
 
    public async Task SayLeaveRequestFulfilledAsync(long userChatId, int requestMessageId, CancellationToken cancellationToken)
    {
        await ApiClient.EditMessageTextAsync(
            chatId: userChatId,
            messageId: requestMessageId,
            text: Options.UserChatMessages.SayUserLeft.SafeFormat(Options.TargetGroupDisplayName),
            parseMode: ParseMode.Html,
            disableWebPagePreview: true,
            cancellationToken: cancellationToken);

        Logger.LogInformation("Said leave request fulfilled in private chat: {UserChatId}, requestMessageId: {RequestMessageId}", userChatId, requestMessageId);
    }

    public async Task TrySayingRequestApprovedAsync(long userChatId, CancellationToken cancellationToken)
    {
        var markup = new InlineKeyboardMarkup(
            InlineKeyboardButton.WithUrl(Options.TargetGroupDisplayName, Options.InvitationLink));

        try
        {
            await ApiClient.SendTextMessageAsync(
                chatId: userChatId,
                text: Options.UserChatMessages.SayRequestApproved,
                parseMode: ParseMode.Html,
                disableWebPagePreview: true,
                replyMarkup: markup,
                cancellationToken: cancellationToken);

            Logger.LogInformation("Said request approved in private chat: {UserChatId}", userChatId);
        }
        catch (ApiRequestException ex)
        {
            // Private chat is available for 24H after join request
            Logger.LogWarning("Cannot say request approved in private chat: {UserChat}, errorCode: {ErrorCode}, errorMessage: {ErrorMessage}", userChatId, ex.ErrorCode, ex.Message);
        }
    }

    public async Task TrySayingRequestDeclinedAsync(long userChatId, CancellationToken cancellationToken)
    {
        var markup = new InlineKeyboardMarkup(
            InlineKeyboardButton.WithCallbackData(Options.UserChatMessages.ButtonAmend, "register"));

        try
        {
            await ApiClient.SendTextMessageAsync(
                chatId: userChatId,
                text: Options.UserChatMessages.SayRequestDeclined,
                parseMode: ParseMode.Html,
                disableWebPagePreview: true,
                replyMarkup: markup,
                cancellationToken: cancellationToken);

            Logger.LogInformation("Said request declined in private chat: {UserChatId}", userChatId);
        }
        catch (ApiRequestException ex)
        {
            // Private chat is available for 24H after join request
            Logger.LogWarning("Cannot say request declined in private chat: {UserChat}, errorCode: {ErrorCode}, errorMessage: {ErrorMessage}", userChatId, ex.ErrorCode, ex.Message);
        }
    }

    public async Task TrySayingUserDeletedAsync(long userChatId, CancellationToken cancellationToken)
    {
        var markup = new InlineKeyboardMarkup(
            InlineKeyboardButton.WithCallbackData(Options.UserChatMessages.ButtonAmend, "register"));

        try
        {
            await ApiClient.SendTextMessageAsync(
                chatId: userChatId,
                text: Options.UserChatMessages.SayUserDeleted,
                parseMode: ParseMode.Html,
                disableWebPagePreview: true,
                replyMarkup: markup,
                cancellationToken: cancellationToken);

            Logger.LogInformation("Said user is deleted in private chat: {UserChatId}", userChatId);
        }
        catch (ApiRequestException ex)
        {
            // Private chat is available for 24H after join request
            Logger.LogWarning("Cannot say user deleted in private chat: {UserChat}, errorCode: {ErrorCode}, errorMessage: {ErrorMessage}", userChatId, ex.ErrorCode, ex.Message);
        }
    }
}

