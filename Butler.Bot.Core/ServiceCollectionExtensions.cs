using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Telegram.Bot;

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

        return services;
    }

    public static IServiceCollection AddButlerBot(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<ButlerOptions>(config);

        services.AddSingleton<IInlineStateManager, InlineStateManager>();

        services.AddSingleton<IWhoisValidator, LengthWhoisValidator>();

        services.AddSingleton<UserChat.IUserChatBot, UserChat.UserChatBot>();
        services.AddSingleton<TargetGroup.ITargetGroupBot, TargetGroup.TargetGroupBot>();
        services.AddSingleton<AdminGroup.IAdminGroupBot, AdminGroup.AdminGroupBot>();

        services.AddSingleton<IUpdateHandler, UserChat.TextMessageHandler>();
        services.AddSingleton<IUpdateHandler, UserChat.RegisterQueryHandler>();
        services.AddSingleton<IUpdateHandler, TargetGroup.JoinRequestHandler>();
        services.AddSingleton<IUpdateHandler, TargetGroup.ChatMemberAddedHandler>();
        services.AddSingleton<IUpdateHandler, AdminGroup.ReviewWhoisHandler>();
        services.AddSingleton<IUpdateHandler, UnknownGroupMessageHandler>();
        services.AddSingleton<IUpdateHandler, BotChatStatusHandler>();
        services.AddSingleton<IUpdateService, UpdateService>();

        return services;
    }
}
