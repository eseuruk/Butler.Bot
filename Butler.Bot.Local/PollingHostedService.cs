using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;

namespace Butler.Bot.Local;

public class PollingHostedService : BackgroundService
{
    private readonly ITelegramBotClient botClient;
    private readonly PollingUpdateHandler updateHandler;
    private readonly ILogger<PollingHostedService> logger;

    public PollingHostedService(ITelegramBotClient botClient, PollingUpdateHandler updateHandler, ILogger<PollingHostedService> logger)
    {
        this.botClient = botClient;
        this.updateHandler = updateHandler;
        this.logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Starting polling service");

        try
        {
            await PollUpdatesAsync(stoppingToken);
        }
        catch(OperationCanceledException)
        {
        }
        catch (Exception ex)
        {
            logger.LogError("Polling service failed with exception: {Exception}", ex);
        }

        logger.LogInformation("Polling service is stoped");
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
