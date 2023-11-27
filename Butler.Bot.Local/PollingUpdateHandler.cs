namespace Butler.Bot.Local;

public class PollingUpdateHandler : Telegram.Bot.Polling.IUpdateHandler
{
    private readonly IUpdateService updateService;
    private readonly ILogger<PollingUpdateHandler> logger;

    public PollingUpdateHandler(IUpdateService updateService, ILogger<PollingUpdateHandler> logger)
    {
        this.updateService = updateService;
        this.logger = logger;
    }

    public Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        return updateService.HandleUpdateAsync(update, cancellationToken);
    }

    public Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        var ErrorMessage = exception switch
        {
            ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        logger.LogInformation("HandleError: {ErrorMessage}", ErrorMessage);

        return Task.CompletedTask;
    }
}
