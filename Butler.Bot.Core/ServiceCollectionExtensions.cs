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

        services.AddSingleton<InlineStateManager>();

        services.AddSingleton<IWhoisValidator, LengthWhoisValidator>();

        services.AddSingleton<UserChat.UserChatBot>();
        services.AddSingleton<TargetGroup.TargetGroupBot>();
        services.AddSingleton<AdminGroup.AdminGroupBot>();
        services.AddSingleton<IButlerBot, ButlerBot>();

        services.AddSingleton<UpdateHandlerBase, UserChat.TextMessageHandler>();
        services.AddSingleton<UpdateHandlerBase, UserChat.RegisterQueryHandler>();
        services.AddSingleton<UpdateHandlerBase, TargetGroup.JoinRequestHandler>();
        services.AddSingleton<UpdateHandlerBase, TargetGroup.ChatMemberAddedHandler>();
        services.AddSingleton<UpdateHandlerBase, AdminGroup.ReviewWhoisHandler>();
        services.AddSingleton<UpdateHandlerBase, UnknownGroupMessageHandler>();
        services.AddSingleton<UpdateHandlerBase, BotChatStatusHandler>();
        services.AddSingleton<IUpdateService, UpdateService>();

        return services;
    }
}
