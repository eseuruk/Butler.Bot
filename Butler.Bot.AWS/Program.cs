using Butler.Bot.Core;
using Butler.Bot.AWS;
using Amazon.DynamoDBv2;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddLambdaLogger(builder.Configuration.GetLambdaLoggerOptions());

builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);

builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
builder.Services.Configure<ConsoleLifetimeOptions>(opts => opts.SuppressStatusMessages = true);
builder.Services.Configure<TelegramApiOptions>(builder.Configuration.GetSection("TelegramApi"));
builder.Services.Configure<ButlerOptions>(builder.Configuration.GetSection("Butler"));
builder.Services.Configure<DynamoUserRepositoryOptions>(builder.Configuration.GetSection("DynamoUserRepository"));

builder.Services.AddTelegramBotClient();

builder.Services.AddAWSService<IAmazonDynamoDB>();
builder.Services.AddSingleton<DynamoJoinRequestTable>();
builder.Services.AddSingleton<IUserRepository, DynamoUserRepository>();

builder.Services.AddSingleton<IWhoisValidator, LengthWhoisValidator>();
builder.Services.AddButlerBot();
builder.Services.AddButlerUpdateService();

builder.Services.AddSingleton<SecretService>();

builder.Services.AddControllers().AddNewtonsoftJson();

builder.Services.AddButlerHealthChecks()
    .AddCheck<DynamoHealthCheck>("DynamoUserRepository")
    .AddCheck<SecretServiceHealthCheck>("SecretService");

var app = builder.Build();

app.MapControllers();

app.Run();