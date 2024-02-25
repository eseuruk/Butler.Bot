namespace Butler.Bot.AWS;

public class WebhookInstaller : IComponentInstaller, IComponentHealthCheck
{
    private readonly ITelegramBotClient apiClient;
    private readonly ILogger<WebhookInstaller> logger;
    private readonly TelegramApiOptions options;

    public string ComponentId => "Webhook";

    public WebhookInstaller(ITelegramBotClient apiClient, ILogger<WebhookInstaller> logger, IOptions<TelegramApiOptions> options)
    {
        this.apiClient = apiClient;
        this.logger = logger;
        this.options = options.Value;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(BotExecutionContext context, CancellationToken cancellationToken)
    {
        try
        {
            var info = await apiClient.GetWebhookInfoAsync(cancellationToken: cancellationToken);

            var expectedUrl = ConstractWebhookUrl(context);
            if (string.IsNullOrEmpty(info.Url))
            {
                return HealthCheckResult.Unhealthy("Webhook is not set");
            }
            else if (!expectedUrl.Equals(info.Url, StringComparison.InvariantCultureIgnoreCase))
            {
                return HealthCheckResult.Unhealthy($"Webhook does not match: {info.Url}, expected: {expectedUrl}");
            }
            else
            {
                return HealthCheckResult.Healthy($"Webhook: {info.Url}, SecretTokenValidation: {options.SecretTokenValidation}");
            }
        }
        catch (ApiRequestException ex)
        {
            logger.LogError(ex, "Error getting webkook info");
            return HealthCheckResult.Unhealthy($"Api error: {ex.ErrorCode} - {ex.Message}");
        }
    }

    public async Task<InstallResult> InstallAsync(BotExecutionContext context, CancellationToken cancellationToken)
    {
        var url = ConstractWebhookUrl(context);
        var secretTocken = options.SecretTokenValidation ? options.SecretToken : null;

        try
        {
            await apiClient.SetWebhookAsync(
                url: url,
                secretToken: secretTocken,
                cancellationToken: cancellationToken);

            return InstallResult.Ok($"Webhook: {url}, SecretTokenValidation: {options.SecretTokenValidation}");
        }
        catch (ApiRequestException ex)
        {
            logger.LogError(ex, "Error installing webkook");
            return InstallResult.Fail($"Api error: {ex.ErrorCode} - {ex.Message}");
        }
    }

    public async Task<InstallResult> UninstallAsync(BotExecutionContext context, CancellationToken cancellationToken)
    {
        try
        {
            await apiClient.DeleteWebhookAsync(
                dropPendingUpdates: true,
                cancellationToken: cancellationToken);

            return InstallResult.Ok("Webhook removed");
        }
        catch (ApiRequestException ex)
        {
            logger.LogError(ex, "Error uninstalling webkook");
            return InstallResult.Fail($"Api error: {ex.ErrorCode} - {ex.Message}");
        }
    }
    
    private string ConstractWebhookUrl(BotExecutionContext context)
    {
        return options.WebhookUrl.Replace("{host}", context.RootUrl);
    }
}