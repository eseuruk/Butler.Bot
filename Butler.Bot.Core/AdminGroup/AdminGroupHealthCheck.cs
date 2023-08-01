using Butler.Bot.Core.TargetGroup;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using System.Diagnostics.Metrics;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.Enums;

namespace Butler.Bot.Core.AdminGroup;

public class AdminGroupHealthCheck : IHealthCheck
{
    private readonly ButlerBot butler;
    private readonly ILogger<TargetGroupHealthCheck> logger;

    public AdminGroupHealthCheck(ButlerBot butler, ILogger<TargetGroupHealthCheck> logger)
    {
        this.butler = butler;
        this.logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        if (butler.Options.WhoisReviewMode == WhoisReviewMode.None)
        {
            return HealthCheckResult.Healthy($"Admin group is not used. whoisReviewMode: {butler.Options.WhoisReviewMode}");
        }

        try
        {
            var me = await butler.ApiClient.GetMeAsync(cancellationToken);
            var member = await butler.ApiClient.GetChatMemberAsync(butler.Options.AdminGroupId, me.Id, cancellationToken);

            logger.LogInformation("Admin group bot membership: {Status}", member.Status);

            if (member.Status != ChatMemberStatus.Administrator)
            {
                return HealthCheckResult.Degraded($"Current bot group status: {member.Status}");
            }
            else
            {
                return HealthCheckResult.Healthy("Bot is an administator in the group");
            }
        }
        catch (ApiRequestException ex)
        {
            logger.LogError(ex, "Error getting bot membership for the admin group");
            return HealthCheckResult.Unhealthy($"Api error: {ex.ErrorCode} - {ex.Message}");
        }
    }
}