﻿using Butler.Bot.Core.TargetGroup;

namespace Butler.Bot.Core.AdminGroup;

public class AdminGroupHealthCheck : IComponentHealthCheck
{
    private readonly ITelegramBotClient apiClient;
    private readonly ButlerOptions options;
    private readonly ILogger<TargetGroupHealthCheck> logger;

    public AdminGroupHealthCheck(ITelegramBotClient apiClient, IOptions<ButlerOptions> options, ILogger<TargetGroupHealthCheck> logger)
    {
        this.apiClient = apiClient;
        this.options = options.Value;
        this.logger = logger;
    }

    public string ComponentId => "AdminGroupMembership";

    public async Task<HealthCheckResult> CheckHealthAsync(BotExecutionContext context, CancellationToken cancellationToken)
    {
        if (options.WhoisReviewMode == WhoisReviewMode.None)
        {
            return HealthCheckResult.Healthy($"Admin group is not used. whoisReviewMode: {options.WhoisReviewMode}");
        }

        try
        {
            var me = await apiClient.GetMeAsync(cancellationToken);
            var member = await apiClient.GetChatMemberAsync(options.AdminGroupId, me.Id, cancellationToken);

            logger.LogInformation("Admin group bot membership: {Status}", member.Status);

            var goodMembership = new[] { ChatMemberStatus.Administrator, ChatMemberStatus.Member };

            var healthStatus = goodMembership.Contains(member.Status) ? HealthStatus.Healthy : HealthStatus.Degraded;

            return new HealthCheckResult(healthStatus, $"Current bot group status: {member.Status}");
        }
        catch (ApiRequestException ex)
        {
            logger.LogError(ex, "Error getting bot membership for the admin group");
            return HealthCheckResult.Unhealthy($"Api error: {ex.ErrorCode} - {ex.Message}");
        }
    }
}