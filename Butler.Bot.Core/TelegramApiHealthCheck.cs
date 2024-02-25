namespace Butler.Bot.Core;

public class TelegramApiHealthCheck : IComponentHealthCheck
{
    private readonly ITelegramBotClient botClient;
    private readonly ILogger<TelegramApiHealthCheck> logger;

    public TelegramApiHealthCheck(ITelegramBotClient botClient, ILogger<TelegramApiHealthCheck> logger)
    {
        this.botClient = botClient;
        this.logger = logger;
    }

    public string ComponentId => "TelegramApi";

    public async Task<HealthCheckResult> CheckHealthAsync(BotExecutionContext context, CancellationToken cancellationToken)
    {
        try
        {
            var me = await botClient.GetMeAsync(cancellationToken);

            logger.LogInformation("Bot information: {Username}, id: {Id}, canJoinGroups: {CanJoinGroups}, canReadAllGroupMessages: {CanReadAllGroupMessages}", me.Username, me.Id, me.CanJoinGroups, me.CanReadAllGroupMessages);
            return HealthCheckResult.Healthy($"Bot name: {me.Username}");
        }
        catch (ApiRequestException ex)
        {
            logger.LogError(ex, "Error getting bot information");
            return HealthCheckResult.Unhealthy($"Api error: {ex.ErrorCode} - {ex.Message}");
        }
    }
}
