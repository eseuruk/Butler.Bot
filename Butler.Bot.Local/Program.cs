using Butler.Bot.Core;
using Butler.Bot.Local;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSerilog((services, loggerConfiguration) => loggerConfiguration
            .WriteTo.Console());

builder.Services.Configure<ConsoleLifetimeOptions>(opts => opts.SuppressStatusMessages = true);
builder.Services.Configure<TelegramApiOptions>(builder.Configuration.GetSection("TelegramApi"));
builder.Services.Configure<ButlerOptions>(builder.Configuration.GetSection("Butler"));

builder.Services.AddTelegramBotClient();
builder.Services.AddSingleton<IUserRepository, InMemoryRequestRepository>();
builder.Services.AddSingleton<IWhoisValidator, LengthWhoisValidator>();
builder.Services.AddButlerBot();
builder.Services.AddButlerUpdateService();
builder.Services.AddButlerHealthChecks();

builder.Services.AddSingleton<PollingUpdateHandler>();
builder.Services.AddHostedService<PollingHostedService>();

var host = builder.Build();

host.Run();
