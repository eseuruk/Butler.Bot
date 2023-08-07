using Butler.Bot.Core;
using Butler.Bot.AWS;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddLambdaLogger(builder.Configuration.GetLambdaLoggerOptions());

builder.Services.Configure<ConsoleLifetimeOptions>(opts => opts.SuppressStatusMessages = true);

builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);
builder.Services.AddDefaultAWSOptions(_ => builder.Configuration.GetAWSOptions());

builder.Services.AddTelegramBotClient(builder.Configuration.GetSection("TelegramApi"));
builder.Services.AddDynamoUserRepository(builder.Configuration.GetSection("DynamoUserRepository"));
builder.Services.AddButlerBot(builder.Configuration.GetSection("Butler"));

builder.Services.AddSingleton<SecretService>();
builder.Services.AddControllers().AddNewtonsoftJson();

builder.Services.AddHealthChecks()
    .AddTelegramApiCheck()
    .AddButlerBotCheck()
    .AddDynamoHealthCheck()
    .AddSecretServiceCheck();

var app = builder.Build();

app.MapControllers();

app.Run();

public partial class Program {}