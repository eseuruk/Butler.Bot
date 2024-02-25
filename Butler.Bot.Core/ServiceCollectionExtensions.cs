namespace Butler.Bot.Core;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTelegramBotClient(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<TelegramApiOptions>(config);

        services.AddSingleton<ITelegramBotClient>( sp =>
        {
            var apiOptions = sp.GetRequiredService<IOptions<TelegramApiOptions>>();

            return new TelegramBotClient(apiOptions.Value.BotToken);
        });

        services.AddSingleton<IComponentHealthCheck, TelegramApiHealthCheck>();

        return services;
    }

    public static IServiceCollection AddButlerBot(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<ButlerOptions>(config);

        services.AddSingleton<IInlineStateManager, InlineStateManager>();

        services.AddSingleton<IWhoisValidator, LengthWhoisValidator>();

        services.AddSingleton<AdminGroup.IAdminGroupMentionStrategy, AdminGroup.AdminGroupMentionStrategy>();
        services.AddSingleton<TargetGroup.ITargetGroupMentionStrategy, TargetGroup.TargetGroupMentionStrategy>();

        services.AddSingleton<UserChat.IUserChatBot, UserChat.UserChatBot>();
        services.AddSingleton<TargetGroup.ITargetGroupBot, TargetGroup.TargetGroupBot>();
        services.AddSingleton<AdminGroup.IAdminGroupBot, AdminGroup.AdminGroupBot>();

        services.AddSingleton<IUpdateHandler, UserChat.TextMessageHandler>();
        services.AddSingleton<IUpdateHandler, UserChat.RegisterQueryHandler>();
        services.AddSingleton<IUpdateHandler, UserChat.LeaveQueryHandler>();
        services.AddSingleton<IUpdateHandler, TargetGroup.JoinRequestHandler>();
        services.AddSingleton<IUpdateHandler, TargetGroup.ChatMemberAddedHandler>();
        services.AddSingleton<IUpdateHandler, AdminGroup.PreJoinWhoisReviewHandler>();
        services.AddSingleton<IUpdateHandler, AdminGroup.PostJoinWhoisReviewHandler>();
        services.AddSingleton<IUpdateHandler, UnknownGroupMessageHandler>();
        services.AddSingleton<IUpdateHandler, BotChatStatusHandler>();
        services.AddSingleton<IUpdateService, UpdateService>();

        services.AddSingleton<IHealthCheckService, HealthCheckService>();
        services.AddSingleton<IInstallService, InstallService>();

        services.AddSingleton<IComponentInstaller, UserChat.UserChatMenuInstaller>();
        services.AddSingleton<IComponentHealthCheck, UserChat.UserChatMenuInstaller>();
        services.AddSingleton<IComponentHealthCheck, TargetGroup.TargetGroupHealthCheck>();
        services.AddSingleton<IComponentHealthCheck, AdminGroup.AdminGroupHealthCheck>();

        return services;
    }
}
