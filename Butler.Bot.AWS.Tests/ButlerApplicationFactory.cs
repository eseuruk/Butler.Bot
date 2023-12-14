using Microsoft.AspNetCore.Hosting;

namespace Butler.Bot.AWS.Tests;

public class ButlerApplicationFactory : WebApplicationFactory<Program>
{
    public TelegramApiOptions TelegramApiOptions { get; } = new TelegramApiOptions();

    public ButlerApplicationFactory()
    {
        ClientOptions.AllowAutoRedirect = false;

        TelegramApiOptions.BotToken = "TEST_BOT_TOKEN";
        TelegramApiOptions.SecretToken = "TEST_SECRET_TOCKEN";
        TelegramApiOptions.SecretTokenValidation = true;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            RecofigureTelegramApi(services);
            RecofigureUserRepository(services);
        });
    }

    private void RecofigureTelegramApi(IServiceCollection services)
    {
        services.Configure<TelegramApiOptions>(option =>
        {
            option.BotToken = TelegramApiOptions.BotToken;
            option.SecretTokenValidation = TelegramApiOptions.SecretTokenValidation;
            option.SecretToken = TelegramApiOptions.SecretToken;
        });

        services.AddSingleton<ITelegramBotClient>(sp =>
        {
            var apiOptions = sp.GetRequiredService<IOptions<TelegramApiOptions>>();

            var testMessageHandler = new TestMessageHandler();
            var httpClient = new HttpClient(testMessageHandler);

            return new TelegramBotClient(apiOptions.Value.BotToken, httpClient);
        });

    }

    private void RecofigureUserRepository(IServiceCollection services)
    {
        services.AddSingleton<IUserRepository, TestRequestRepository>();
    }
}
