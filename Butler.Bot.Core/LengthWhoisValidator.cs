using System.Globalization;

namespace Butler.Bot.Core;

public class LengthWhoisValidator : IWhoisValidator
{
    private readonly ButlerOptions options;
    private readonly ILogger<LengthWhoisValidator> logger;

    public LengthWhoisValidator(IOptions<ButlerOptions> options, ILogger<LengthWhoisValidator> logger)
    {
        this.options = options.Value;
        this.logger = logger;
    }

    public (bool, string) CheckMessageText(string messageText)
    {
        var stringIfo = new StringInfo(messageText);
        int whoisLength = stringIfo.LengthInTextElements;

        bool result = whoisLength >= options.MinWoisLength;

        logger.LogInformation("Whois length check. result: {Result} length: {WhoisLength} message: {Message}", result, whoisLength, messageText);

        var error = result ? string.Empty : options.UserChatMessages.SayWhoisTooShort.SafeFormat(whoisLength, options.MinWoisLength);
        
        return (result, error);
    }
}


