namespace Butler.Bot.Core.TargetGroup;

public class TargetGroupHealthCheck : IHealthCheck
{
    private readonly ITelegramBotClient apiClient;
    private readonly ButlerOptions options;
    private readonly ILogger<TargetGroupHealthCheck> logger;

    public TargetGroupHealthCheck(ITelegramBotClient apiClient, IOptions<ButlerOptions> options, ILogger<TargetGroupHealthCheck> logger)
    {
        this.apiClient = apiClient;
        this.options = options.Value;
        this.logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var me = await apiClient.GetMeAsync(cancellationToken);
            var member = await apiClient.GetChatMemberAsync(options.TargetGroupId, me.Id, cancellationToken);

            logger.LogInformation("Target group bot membership: {Status}", member.Status);

            var healthStatus = member.Status == ChatMemberStatus.Administrator ? HealthStatus.Healthy : HealthStatus.Degraded;

            return new HealthCheckResult(healthStatus, $"Current bot group status: {member.Status}");
        }
        catch (ApiRequestException ex)
        {
            logger.LogError(ex, "Error getting bot membership for the target group");
            return HealthCheckResult.Unhealthy($"Api error: {ex.ErrorCode} - {ex.Message}");
        }
    }
}