using Butler.Bot.Local;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSerilog((services, loggerConfiguration) => loggerConfiguration
            .WriteTo.Console());

builder.Services.Configure<ConsoleLifetimeOptions>(opts => opts.SuppressStatusMessages = true);

builder.Services.AddTelegramBotClient(builder.Configuration.GetSection("TelegramApi"));
builder.Services.AddUserRepository(builder.Configuration.GetSection("UserRepository"));
builder.Services.AddButlerBot(builder.Configuration.GetSection("Butler"));

builder.Services.AddSingleton<PollingUpdateHandler>();
builder.Services.AddHostedService<PollingHostedService>();

var host = builder.Build();

host.Run();
