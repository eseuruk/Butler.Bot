using Telegram.Bot.Polling;

namespace Butler.Bot.Local;

public class PollingHostedService : BackgroundService
{
    private readonly IHealthCheckService healthService;
    private readonly ITelegramBotClient botClient;
    private readonly PollingUpdateHandler updateHandler;
    private readonly ILogger<PollingHostedService> logger;

    public PollingHostedService(IHealthCheckService healthService, ITelegramBotClient botClient, PollingUpdateHandler updateHandler, ILogger<PollingHostedService> logger)
    {
        this.healthService = healthService;
        this.botClient = botClient;
        this.updateHandler = updateHandler;
        this.logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Starting polling service");

        try
        {
            bool systemOk = await CheckSystemStatusAsync(stoppingToken);
            if (systemOk)
            {
                await PollUpdatesAsync(stoppingToken);
            }
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogCritical(ex, "Polling service failed with exception");
            return;
        }

        logger.LogInformation("Polling service is stoped");
    }

    private async Task<bool> CheckSystemStatusAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Checking system health");

        var context = new BotExecutionContext();
        var filter = new ComponentFilter(null);

        var healthReport = await healthService.CheckHealthAsync(context, filter, stoppingToken);

        if (healthReport.Status == HealthStatus.Unhealthy)
        {
            logger.LogInformation("System health: {Status}. Execution aborted", healthReport.Status);
            return false;
        }
        else
        {
            logger.LogInformation("System health: {Status}. Continue execution", healthReport.Status);
            return true;
        }
    }

    private async Task PollUpdatesAsync(CancellationToken stoppingToken)
    {
        var receiverOptions = new ReceiverOptions()
        {
            AllowedUpdates = Array.Empty<UpdateType>(),
            ThrowPendingUpdates = true,
        };

        var me = await botClient.GetMeAsync(stoppingToken);

        logger.LogInformation("Start receiving updates for {BotName}", me.Username );

        await botClient.ReceiveAsync(
            updateHandler: updateHandler,
            receiverOptions: receiverOptions,
            cancellationToken: stoppingToken);
    }
}
