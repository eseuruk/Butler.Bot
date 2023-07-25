using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using System.Globalization;
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
                InlineKeyboardButton.WithCallbackData("Приступить", "register"));

        await apiClient.SendTextMessageAsync(
            chatId: userChatId,
            text: $"Привет! Я помогу тебя зайти в группу {options.TargetGroupDisplayName}. Если не против, я задам тебе несколько вопросов, перед тем как ты сможешь туда попасть.",
            replyMarkup: markup,
            cancellationToken: cancellationToken);

        logger.LogInformation("Said hello in private chat: {UserChatId}", userChatId);
    }

    public async Task SayConfusedAsync(long userChatId, CancellationToken cancellationToken)
    {
        var markup = new InlineKeyboardMarkup(
            InlineKeyboardButton.WithCallbackData("Продолжить", "register"));

        await apiClient.SendTextMessageAsync(
            chatId: userChatId,
            text: $"Наверное ты хочешь зайти в группу {options.TargetGroupDisplayName}. Если да, то нажми на кнопку ниже чтоб продолжить.",
            replyMarkup: markup,
            cancellationToken: cancellationToken);

        logger.LogInformation("Said confused in private chat: {UserChatId}", userChatId);
    }

    public async Task AskForWhoisAsync(long userChatId, CancellationToken cancellationToken)
    {
        await apiClient.SendTextMessageAsync(
            chatId: userChatId,
            text: $"Пожалуйста, представься как тебя зовут, чем занимаешься и увлекаешься, что тебя привело в этот чат (минимум {options.MinWoisLength} символов)",
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
            text: "Спасибо за детальное описание. Теперь, нажми на кнопку ниже и запроси доступ к группе.",
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
            text: "Похоже, ты состоял в групе раньше но вышел из нее. Пожалуйста, нажми на кнопку ниже и запроси доступ снова.",
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
            text: "Ты уже состоишь в нашей группе. Нажми на кнопну ниже, чтобы перейти в нее.",
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
                text: "Запрос в группу был одобрен. Нажми на кнопку ниже чтобы перейти в нее.",
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
            InlineKeyboardButton.WithCallbackData("Исправить", "register"));

        try
        {
            await apiClient.SendTextMessageAsync(
                chatId: userChatId,
                text: "К сожалению, запрос в группу был отклонен. Попробуй исправить описание и запросить снова.",
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
            InlineKeyboardButton.WithCallbackData("Исправить", "register"));

        try
        {
            await apiClient.SendTextMessageAsync(
                chatId: userChatId,
                text: "К сожалению, администраторы удалили тебя из группы из-за плохого описания. Попробуй исправить его и запросить доступ снова.",
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

