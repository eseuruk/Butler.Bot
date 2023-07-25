using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Telegram.Bot;

namespace Butler.Bot.Core;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTelegramBotClient(this IServiceCollection services)
    {
        services.AddSingleton<ITelegramBotClient>( sp =>
        {
            var apiOptions = sp.GetRequiredService<IOptions<TelegramApiOptions>>();

            return new TelegramBotClient(apiOptions.Value.BotToken);
        });

        return services;
    }

    public static IServiceCollection AddButlerUpdateService(this IServiceCollection services)
    {
        services.AddSingleton<UpdateHandlerBase, UserChat.TextMessageHandler>();
        services.AddSingleton<UpdateHandlerBase, UserChat.RegisterQueryHandler>();

        services.AddSingleton<UpdateHandlerBase, TargetGroup.JoinRequestHandler>();
        services.AddSingleton<UpdateHandlerBase, TargetGroup.ChatMemberAddedHandler>();

        services.AddSingleton<UpdateHandlerBase, AdminGroup.ReviewWhoisHandler>();

        services.AddSingleton<UpdateHandlerBase, UnknownGroupMessageHandler>();

        services.AddSingleton<UpdateService>();

        return services;
    }

    public static IServiceCollection AddButlerBot(this IServiceCollection services)
    {
        services.AddSingleton<UserChat.UserChatBot>();
        services.AddSingleton<TargetGroup.TargetGroupBot>();
        services.AddSingleton<AdminGroup.AdminGroupBot>();
        services.AddSingleton<ButlerBot>();

        return services;
    }


}
