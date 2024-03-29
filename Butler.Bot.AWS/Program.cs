using Butler.Bot.AWS;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddLambdaLogger(builder.Configuration.GetLambdaLoggerOptions());

builder.Services.Configure<ConsoleLifetimeOptions>(opts => opts.SuppressStatusMessages = true);
builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);

builder.Services.AddTelegramBotClient(builder.Configuration.GetSection("TelegramApi"));
builder.Services.AddDynamoUserRepository(builder.Configuration.GetSection("DynamoUserRepository"));
builder.Services.AddButlerBot(builder.Configuration.GetSection("Butler"));

builder.Services.AddSingleton<SecretService>();
builder.Services.AddControllers().AddNewtonsoftJson();

builder.Services.AddSingleton<IComponentInstaller, WebhookInstaller>();
builder.Services.AddSingleton<IComponentHealthCheck, WebhookInstaller>();

var app = builder.Build();

app.MapControllers();

app.Run();

// To make Butler.Bot.AWS.Tests working
public partial class Program {}