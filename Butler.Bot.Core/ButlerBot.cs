﻿using Butler.Bot.Core.AdminGroup;
using Butler.Bot.Core.UserChat;
using Butler.Bot.Core.TargetGroup;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Exceptions;

namespace Butler.Bot.Core;

public interface IButlerBot
{
    ITelegramBotClient ApiClient { get; init; }
    ButlerOptions Options { get; init; }
    UserChatBot UserChat { get; init; }
    TargetGroupBot TargetGroup { get; init; }
    AdminGroupBot AdminGroup { get; init; }
    Task StopQuerySpinnerAsync(string callbackQueryId, CancellationToken cancellationToken);
}

public class ButlerBot : IButlerBot
{
    private readonly ILogger<ButlerBot> logger;

    public ButlerBot(ITelegramBotClient apiClient, IOptions<ButlerOptions> options, UserChatBot privateChat, TargetGroupBot targetGroup, AdminGroupBot adminGroup, ILogger<ButlerBot> logger)
    {
        ApiClient = apiClient;
        Options = options.Value;
        UserChat = privateChat;
        TargetGroup = targetGroup;
        AdminGroup = adminGroup;

        this.logger = logger;
    }

    public ITelegramBotClient ApiClient { get; init; }

    public ButlerOptions Options { get; init; }

    public UserChatBot UserChat { get; init; }

    public TargetGroupBot TargetGroup { get; init; }

    public AdminGroupBot AdminGroup { get; init; }

    public async Task StopQuerySpinnerAsync(string callbackQueryId, CancellationToken cancellationToken)
    {
        try
        {
            // Answer callback with no data to stop pending status on client side
            await ApiClient.AnswerCallbackQueryAsync(callbackQueryId, cancellationToken: cancellationToken);
        }
        catch(ApiRequestException ex)
        {
            // Callback might expire during retries so catch exceptions to unblock update processing
            logger.LogWarning("Cannot answer query callback to: {callbackQueryId}, errorCode: {ErrorCode}, errorMessage: {ErrorMessage}", callbackQueryId, ex.ErrorCode, ex.Message);
        }
    }
}
