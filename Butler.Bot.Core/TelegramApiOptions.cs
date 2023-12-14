namespace Butler.Bot.Core;

public class TelegramApiOptions
{
    public string BotToken { get; set; } = String.Empty;

    public string SecretToken { get; set; } = String.Empty;

    public bool SecretTokenValidation { get; set; }
}
