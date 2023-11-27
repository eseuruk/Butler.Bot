namespace Butler.Bot.AWS;

public class SecretService
{
    private readonly TelegramApiOptions options;
    private ILogger<SecretService> logger;

    public SecretService(IOptions<TelegramApiOptions> options, ILogger<SecretService> logger)
    {
        this.options = options.Value;
        this.logger = logger;
    }

    public bool ValidateToken(string? token)
    {
        if (!options.SecretTokenValidation)
        {
            logger.LogInformation("Secret token validation if off");
            return true;
        }
        else if (options.SecretToken == token)
        {
            logger.LogInformation("Secret token is valid");
            return true;
        }
        else
        {
            logger.LogCritical("Secret token is invalid: {token}", token);
            return false;
        }
    }
}

