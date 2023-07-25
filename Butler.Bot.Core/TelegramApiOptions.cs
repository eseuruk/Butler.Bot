namespace Butler.Bot.Core;

public class TelegramApiOptions
{
    public string BotToken { get; init; } = String.Empty;

    public string SecretToken { get; init; } = String.Empty;

    public bool SecretTokenValidation { get; init; }
}
