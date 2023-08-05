using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.Enums;

namespace Butler.Bot.Core.TargetGroup;

public class TargetGroupHealthCheck : IHealthCheck
{
    private readonly ButlerBot butler;
    private readonly ILogger<TargetGroupHealthCheck> logger;

    public TargetGroupHealthCheck(ButlerBot butler, ILogger<TargetGroupHealthCheck> logger)
    {
        this.butler = butler;
        this.logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var me = await butler.ApiClient.GetMeAsync(cancellationToken);
            var member = await butler.ApiClient.GetChatMemberAsync(butler.Options.TargetGroupId, me.Id, cancellationToken);

            logger.LogInformation("Target group bot membership: {Status}", member.Status);

            if (member.Status != ChatMemberStatus.Administrator)
            {
                return HealthCheckResult.Degraded($"Current bot group status: {member.Status}");
            }
            else
            {
                return HealthCheckResult.Healthy("Bot is a administator in the group");
            }
        }
        catch (ApiRequestException ex)
        {
            logger.LogError(ex, "Error getting bot membership for the target group");
            return HealthCheckResult.Unhealthy($"Api error: {ex.ErrorCode} - {ex.Message}");
        }
    }
}