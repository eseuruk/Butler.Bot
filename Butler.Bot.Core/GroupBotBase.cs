namespace Butler.Bot.Core;

public abstract class GroupBotBase
{
    private readonly ITelegramBotClient apiClient;
    private readonly ButlerOptions options;

    private readonly ILogger logger;

    public GroupBotBase(ITelegramBotClient apiClient, IOptions<ButlerOptions> options, ILogger logger)
    {
        this.apiClient = apiClient;
        this.options = options.Value;
        this.logger = logger;
    }

    protected ITelegramBotClient ApiClient { get { return apiClient; } }

    protected ButlerOptions Options { get { return options; } }

    protected ILogger Logger { get { return logger; } }

    public async Task StopQuerySpinnerAsync(string callbackQueryId, CancellationToken cancellationToken)
    {
        try
        {
            // Answer callback with no data to stop pending status on client side
            await ApiClient.AnswerCallbackQueryAsync(callbackQueryId, cancellationToken: cancellationToken);
        }
        catch (ApiRequestException ex)
        {
            // Callback might expire during retries so catch exceptions to unblock update processing
            Logger.LogWarning("Cannot answer query callback to: {callbackQueryId}, errorCode: {ErrorCode}, errorMessage: {ErrorMessage}", callbackQueryId, ex.ErrorCode, ex.Message);
        }
    }
}

