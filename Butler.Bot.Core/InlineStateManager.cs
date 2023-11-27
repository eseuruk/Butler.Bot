using System.Text;

namespace Butler.Bot.Core;

public class InlineStateManager : IInlineStateManager
{
    // Implementation idea get from https://github.com/nmlorg/metabot/issues/1

    private const string urlPrefix = "tg://btn/";

    private readonly ILogger<InlineStateManager> logger;

    public InlineStateManager(ILogger<InlineStateManager> logger)
    {
        this.logger = logger;
    }

    public string InjectStateIntoMessageHtml<T>(string messageHtml, T state)
    {
        var serializedState = SerializeState(state);
        var encodedState = Base64Encode(serializedState);
        var stateLinkHtml = CreateStateLinkHtml(encodedState);

        logger.LogInformation("Inline state link created: {StateLink}", stateLinkHtml);

        return stateLinkHtml + messageHtml;
    }

    public T GetStateFromMessage<T>(Message message)
    {
        var encodedState = GetStateLinkFromMessage(message);
        logger.LogInformation("Inline state loaded from the messege: {Id} state: {State}", message.MessageId, encodedState);

        var serializedState = Base64Decode(encodedState);
        return DeserializeState<T>(serializedState);
    }

    private string CreateStateLinkHtml(string encodedState)
    {
        return $"<a href='{urlPrefix}{encodedState}'>\u200b</a>";
    }

    private string GetStateLinkFromMessage(Message message)
    {
        // state link is stored in entities as text link with predefined prefix

        if (message.Entities != null)
            foreach (var entry in message.Entities)
            {
                if (entry.Type == MessageEntityType.TextLink &&
                    entry.Url != null && entry.Url.StartsWith(urlPrefix))
                {
                    return entry.Url.Substring(urlPrefix.Length);
                }
            }

        throw new InlineStateException("Inline state link is not found in the message. Format might be chaged.");
    }

    private string SerializeState<T>(T state)
    {
        return JsonConvert.SerializeObject(state);
    }

    private T DeserializeState<T>(string serializedState)
    {
        try
        {
            var desiarized = JsonConvert.DeserializeObject<T>(serializedState);
            if (desiarized == null)
            {
                throw new InlineStateException($"Error deserializing state: {serializedState}");
            }
            return desiarized;
        }
        catch (JsonException ex)
        {
            throw new InlineStateException($"Error deserializing state: {serializedState}", ex);
        }
    }

    private string Base64Encode(string plainText)
    {
        var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
        return Convert.ToBase64String(plainTextBytes);
    }

    private string Base64Decode(string base64EncodedData)
    {
        try
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }
        catch (FormatException ex)
        {
            throw new InlineStateException($"Error parsing base64: {base64EncodedData}", ex);
        }
    }
}
