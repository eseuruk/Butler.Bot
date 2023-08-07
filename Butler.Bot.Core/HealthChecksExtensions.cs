using Microsoft.Extensions.DependencyInjection;

namespace Butler.Bot.Core;

public static class HealthChecksExtensions
{
    public static IHealthChecksBuilder AddTelegramApiCheck(this IHealthChecksBuilder builder)
    {
        return builder.AddCheck<TelegramApiHealthCheck>("TelegramApi");
    }

    public static IHealthChecksBuilder AddButlerBotCheck(this IHealthChecksBuilder builder)
    {
        return builder
            .AddCheck<TargetGroup.TargetGroupHealthCheck>("TargetGroupMembership")
            .AddCheck<AdminGroup.AdminGroupHealthCheck>("AdminGroupMembership");
    }
}
