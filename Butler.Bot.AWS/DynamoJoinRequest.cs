using Amazon.DynamoDBv2.DataModel;

namespace Butler.Bot.AWS;

[DynamoDBTable("joinRequests")]
public class DynamoJoinRequest
{
    [DynamoDBHashKey("id")]
    public long UserId { get; set; }

    [DynamoDBProperty("whois")]
    public string Whois { get; set; } = string.Empty;

    [DynamoDBProperty("whois_messagem_id")]
    public int WhoisMessageId { get; set; }

    [DynamoDBProperty("user_chat_id")]
    public long UserChatId { get; set; }
}
