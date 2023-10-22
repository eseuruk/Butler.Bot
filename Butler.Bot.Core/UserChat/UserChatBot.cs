﻿using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Exceptions;

namespace Butler.Bot.Core.UserChat;

public class UserChatBot : GroupBotBase, IUserChatBot
{
    public UserChatBot(ITelegramBotClient apiClient, IOptions<ButlerOptions> options, ILogger<UserChatBot> logger)
        : base(apiClient, options, logger)
    {
    }

    public async Task SayBotVersionAsync(long userChatId, CancellationToken cancellationToken)
    {
        var botVersion = ButlerVersion.GetCurrent();

        await ApiClient.SendTextMessageAsync(
            chatId: userChatId,
            text: Options.UserChatMessages.SayBotVersion.SafeFormat(botVersion),
            cancellationToken: cancellationToken);

        Logger.LogInformation("Said current version in private chat: {UserChatId} version: {Version}", userChatId, botVersion);
    }

    public async Task SayHelloAsync(long userChatId, CancellationToken cancellationToken)
    {
        var markup = new InlineKeyboardMarkup(
                InlineKeyboardButton.WithCallbackData(Options.UserChatMessages.ButtonApply, "register"));

        await ApiClient.SendTextMessageAsync(
            chatId: userChatId,
            text: Options.UserChatMessages.SayHello.SafeFormat(Options.TargetGroupDisplayName),
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
            replyMarkup: markup,
            cancellationToken: cancellationToken);

        Logger.LogInformation("Said confused in private chat: {UserChatId}", userChatId);
    }

    public async Task AskForWhoisAsync(long userChatId, CancellationToken cancellationToken)
    {
        await ApiClient.SendTextMessageAsync(
            chatId: userChatId,
            text: Options.UserChatMessages.AskForWhois.SafeFormat(Options.MinWoisLength),
            cancellationToken: cancellationToken);

        Logger.LogInformation("Asked to provide whois in private chat: {UserChatId}", userChatId);
    }

    public async Task WarnWhoisValidationFailedAsync(long userChatId, string validationError, CancellationToken cancellationToken)
    {
        await ApiClient.SendTextMessageAsync(
            chatId: userChatId,
            text: validationError,
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
            replyMarkup: markup,
            cancellationToken: cancellationToken);

        Logger.LogInformation("Said whois is ok in private chat and ask to join target group. private chat: {UserChatId}", userChatId);
    }

    public async Task SayUsedToBeMemberAsync(long userChatId, CancellationToken cancellationToken)
    {
        var markup = new InlineKeyboardMarkup(
                InlineKeyboardButton.WithUrl(Options.TargetGroupDisplayName, Options.InvitationLink));

        await ApiClient.SendTextMessageAsync(
            chatId: userChatId,
            text: Options.UserChatMessages.SayUsedToBeMember,
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
            replyMarkup: markup,
            cancellationToken: cancellationToken);

        Logger.LogInformation("Said already a member in private chat: {UserChatId}", userChatId);
    }

    public async Task SayBlockedAsync(long userChatId, CancellationToken cancellationToken)
    {
        await ApiClient.SendTextMessageAsync(
            chatId: userChatId,
            text: Options.UserChatMessages.SayBlockedMember,
            cancellationToken: cancellationToken);

        Logger.LogInformation("Said blocked in private chat: {UserChatId}", userChatId);
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

