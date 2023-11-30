namespace Butler.Bot.DynamoDB;

public class DynamoUserRepositoryOptions
{
    // if null then use AWS_REGION from execution context
    public string? Region { get; init; }

    public string Table { get; init; } = "joinRequests";

    public string UserId { get; init; } = "id";

    public string Whois { get; init; } = "whois";

    public string WhoisMessageId { get; init; } = "whois_messagem_id";

    public string UserChatId { get; init; } = "user_chat_id";
}
