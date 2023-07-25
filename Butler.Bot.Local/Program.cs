using Butler.Bot.Core;
using Butler.Bot.Local;
using Serilog;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices((context, services) =>
    {
        services.AddSerilog((services, loggerConfiguration) => loggerConfiguration
            .WriteTo.Console());

        services.Configure<TelegramApiOptions>(context.Configuration.GetSection("TelegramApi"));
        services.Configure<ButlerOptions>(context.Configuration.GetSection("Butler"));

        services.AddTelegramBotClient();
        services.AddSingleton<IUserRepository, InMemoryRequestRepository>();
        services.AddSingleton<IWhoisValidator, LengthWhoisValidator>();
        services.AddButlerBot();
        services.AddButlerUpdateService();

        services.AddSingleton<PollingUpdateHandler>();
        services.AddHostedService<PollingHostedService>();
    });

var host = builder.Build();

await host.RunAsync();
