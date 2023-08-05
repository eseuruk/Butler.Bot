using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Exceptions;

namespace Butler.Bot.Core.UserChat;

public class UserChatBot
{
    private readonly ITelegramBotClient apiClient;
    private readonly ButlerOptions options;

    private readonly ILogger<UserChatBot> logger;

    public UserChatBot(ITelegramBotClient apiClient, IOptions<ButlerOptions> options, ILogger<UserChatBot> logger)
    {
        this.apiClient = apiClient;
        this.options = options.Value;
        this.logger = logger;
    }

    public async Task SayHelloAsync(long userChatId, CancellationToken cancellationToken)
    {
        var markup = new InlineKeyboardMarkup(
                InlineKeyboardButton.WithCallbackData(options.UserChatMessages.ButtonApply, "register"));

        await apiClient.SendTextMessageAsync(
            chatId: userChatId,
            text: options.UserChatMessages.SayHello.SafeFormat(options.TargetGroupDisplayName),
            replyMarkup: markup,
            cancellationToken: cancellationToken);

        logger.LogInformation("Said hello in private chat: {UserChatId}", userChatId);
    }

    public async Task SayConfusedAsync(long userChatId, CancellationToken cancellationToken)
    {
        var markup = new InlineKeyboardMarkup(
            InlineKeyboardButton.WithCallbackData(options.UserChatMessages.ButtonContinue, "register"));

        await apiClient.SendTextMessageAsync(
            chatId: userChatId,
            text: options.UserChatMessages.SayConfused.SafeFormat(options.TargetGroupDisplayName),
            replyMarkup: markup,
            cancellationToken: cancellationToken);

        logger.LogInformation("Said confused in private chat: {UserChatId}", userChatId);
    }

    public async Task AskForWhoisAsync(long userChatId, CancellationToken cancellationToken)
    {
        await apiClient.SendTextMessageAsync(
            chatId: userChatId,
            text: options.UserChatMessages.AskForWhois.SafeFormat(options.MinWoisLength),
            cancellationToken: cancellationToken);

        logger.LogInformation("Asked to provide whois in private chat: {UserChatId}", userChatId);
    }

    public async Task WarnWhoisValidationFailedAsync(long userChatId, string validationError, CancellationToken cancellationToken)
    {
        await apiClient.SendTextMessageAsync(
            chatId: userChatId,
            text: validationError,
            cancellationToken: cancellationToken);

        logger.LogInformation("Said whois is not valid in private chat: {UserChatId}", userChatId);
    }

    public async Task SayWhoisOkAndAskToRequestAccessAsync(long userChatId, CancellationToken cancellationToken)
    {
        var markup = new InlineKeyboardMarkup(
                InlineKeyboardButton.WithUrl(options.TargetGroupDisplayName, options.InvitationLink));

        await apiClient.SendTextMessageAsync(
            chatId: userChatId,
            text: options.UserChatMessages.SayWhoisOkAndAskToRequestAccess,
            replyMarkup: markup,
            cancellationToken: cancellationToken);

        logger.LogInformation("Said whois is ok in private chat and ask to join target group. private chat: {UserChatId}", userChatId);
    }

    public async Task SayUsedToBeMemberAsync(long userChatId, CancellationToken cancellationToken)
    {
        var markup = new InlineKeyboardMarkup(
                InlineKeyboardButton.WithUrl(options.TargetGroupDisplayName, options.InvitationLink));

        await apiClient.SendTextMessageAsync(
            chatId: userChatId,
            text: options.UserChatMessages.SayUsedToBeMember,
            replyMarkup: markup,
            cancellationToken: cancellationToken);

        logger.LogInformation("Said used to be a member in private chat: {UserChatId}", userChatId);
    }

    public async Task SayAlreadyMemberAsync(long userChatId, CancellationToken cancellationToken)
    {
        var markup = new InlineKeyboardMarkup(
                InlineKeyboardButton.WithUrl(options.TargetGroupDisplayName, options.InvitationLink));

        await apiClient.SendTextMessageAsync(
            chatId: userChatId,
            text: options.UserChatMessages.SayAlreadyMember,
            replyMarkup: markup,
            cancellationToken: cancellationToken);

        logger.LogInformation("Said already a member in private chat: {UserChatId}", userChatId);
    }

    public async Task TrySayingRequestApprovedAsync(long userChatId, CancellationToken cancellationToken)
    {
        var markup = new InlineKeyboardMarkup(
            InlineKeyboardButton.WithUrl(options.TargetGroupDisplayName, options.InvitationLink));

        try
        {
            await apiClient.SendTextMessageAsync(
                chatId: userChatId,
                text: options.UserChatMessages.SayRequestApproved,
                replyMarkup: markup,
                cancellationToken: cancellationToken);

            logger.LogInformation("Said request approved in private chat: {UserChatId}", userChatId);
        }
        catch (ApiRequestException ex)
        {
            // Private chat is available for 24H after join request
            logger.LogWarning("Cannot say request approved in private chat: {UserChat}, errorCode: {ErrorCode}, errorMessage: {ErrorMessage}", userChatId, ex.ErrorCode, ex.Message);
        }
    }

    public async Task TrySayingRequestDeclinedAsync(long userChatId, CancellationToken cancellationToken)
    {
        var markup = new InlineKeyboardMarkup(
            InlineKeyboardButton.WithCallbackData(options.UserChatMessages.ButtonAmend, "register"));

        try
        {
            await apiClient.SendTextMessageAsync(
                chatId: userChatId,
                text: options.UserChatMessages.SayRequestDeclined,
                replyMarkup: markup,
                cancellationToken: cancellationToken);

            logger.LogInformation("Said request declined in private chat: {UserChatId}", userChatId);
        }
        catch (ApiRequestException ex)
        {
            // Private chat is available for 24H after join request
            logger.LogWarning("Cannot say request declined in private chat: {UserChat}, errorCode: {ErrorCode}, errorMessage: {ErrorMessage}", userChatId, ex.ErrorCode, ex.Message);
        }
    }

    public async Task TrySayingUserDeletedAsync(long userChatId, CancellationToken cancellationToken)
    {
        var markup = new InlineKeyboardMarkup(
            InlineKeyboardButton.WithCallbackData(options.UserChatMessages.ButtonAmend, "register"));

        try
        {
            await apiClient.SendTextMessageAsync(
                chatId: userChatId,
                text: options.UserChatMessages.SayUserDeleted,
                replyMarkup: markup,
                cancellationToken: cancellationToken);

            logger.LogInformation("Said user is deleted in private chat: {UserChatId}", userChatId);
        }
        catch (ApiRequestException ex)
        {
            // Private chat is available for 24H after join request
            logger.LogWarning("Cannot say user deleted in private chat: {UserChat}, errorCode: {ErrorCode}, errorMessage: {ErrorMessage}", userChatId, ex.ErrorCode, ex.Message);
        }
    }
}

