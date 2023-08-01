using Butler.Bot.Core.TargetGroup;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Exceptions;

namespace Butler.Bot.Core;

public class TelegramApiHealthCheck : IHealthCheck
{
    private readonly ITelegramBotClient botClient;
    private readonly ILogger<TelegramApiHealthCheck> logger;

    public TelegramApiHealthCheck(ITelegramBotClient botClient, ILogger<TelegramApiHealthCheck> logger)
    {
        this.botClient = botClient;
        this.logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var me = await botClient.GetMeAsync(cancellationToken);

            logger.LogInformation("Bot information: {Username}, id: {Id}, canJoinGroups: {CanJoinGroups}, canReadAllGroupMessages: {CanReadAllGroupMessages}", me.Username, me.Id, me.CanJoinGroups, me.CanReadAllGroupMessages);
            return HealthCheckResult.Healthy($"Api response: {me.Username}");
        }
        catch (ApiRequestException ex)
        {
            logger.LogError(ex, "Error getting bot information");
            return HealthCheckResult.Unhealthy($"Api error: {ex.ErrorCode} - {ex.Message}");
        }
    }
}
