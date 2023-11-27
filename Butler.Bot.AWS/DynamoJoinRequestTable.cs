using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

namespace Butler.Bot.AWS;

public class DynamoJoinRequestTable
{
    private readonly IAmazonDynamoDB client;
    private readonly DynamoUserRepositoryOptions options;
    private readonly ILogger<DynamoJoinRequestTable> logger;


    public DynamoJoinRequestTable(IAmazonDynamoDB client, IOptions<DynamoUserRepositoryOptions> options, ILogger<DynamoJoinRequestTable> logger)
    {
        this.client = client;
        this.options = options.Value;
        this.logger = logger;
    }

    public async Task<JoinRequest?> GetItemAsync(long userId, CancellationToken cancellationToken)
    {
        var key = new Dictionary<string, AttributeValue>();
        Add_Id(key, userId);

        var apiResponse = await client.GetItemAsync(options.Table, key, cancellationToken);
        if (!apiResponse.IsItemSet) return null;

        return new JoinRequest
        {
            UserId = Get_Id(apiResponse.Item),
            Whois = Get_Whois(apiResponse.Item),
            WhoisMessageId = Get_WhoisMessageId(apiResponse.Item),
            UserChatId = Get_UserChatId(apiResponse.Item)
        };
    }

    public async Task PutItemAsync(JoinRequest request, CancellationToken cancellationToken)
    {
        var item = new Dictionary<string, AttributeValue>();

        Add_Id(item, request.UserId);
        Add_Whois(item, request.Whois);
        Add_WhoisMessageId(item, request.WhoisMessageId);
        Add_UserChatId(item, request.UserChatId);

        await client.PutItemAsync(options.Table, item, cancellationToken);
    }

    public async Task DeleteItemAsync(long userId, CancellationToken cancellationToken)
    {
        var key = new Dictionary<string, AttributeValue>();
        Add_Id(key, userId);

        await client.DeleteItemAsync(options.Table, key, cancellationToken);
    }

    private void Add_Id(Dictionary<string, AttributeValue> attributes, long value)
    {
        attributes.Add(options.UserId, new AttributeValue { N = value.ToString() });
    }

    private long Get_Id(Dictionary<string, AttributeValue> attributes)
    {
        if (attributes.TryGetValue(options.UserId, out var value))
        {
            if (long.TryParse(value.N, out var result))
            {
                return result;
            }
            else
            {
                LogParsingError(options.UserId, value.N);
            }
        }
        return 0;
    }

    private void Add_Whois(Dictionary<string, AttributeValue> attributes, string value)
    {
        attributes.Add(options.Whois, new AttributeValue { S = value });
    }

    private string Get_Whois(Dictionary<string, AttributeValue> attributes)
    {
        if (attributes.TryGetValue(options.Whois, out var value))
        {
            return value.S;
        }
        return string.Empty;
    }

    private void Add_WhoisMessageId(Dictionary<string, AttributeValue> attributes, int value)
    {
        attributes.Add(options.WhoisMessageId, new AttributeValue { N = value.ToString() });
    }

    private int Get_WhoisMessageId(Dictionary<string, AttributeValue> attributes)
    {
        if (attributes.TryGetValue(options.WhoisMessageId, out var value))
        {
            if (int.TryParse(value.N, out var result))
            {
                return result;
            }
            else
            {
                LogParsingError(options.WhoisMessageId, value.N);
            }
        }
        return 0;
    }

    private void Add_UserChatId(Dictionary<string, AttributeValue> attributes, long value)
    {
        attributes.Add(options.UserChatId, new AttributeValue { N = value.ToString() });
    }

    private long Get_UserChatId(Dictionary<string, AttributeValue> attributes)
    {
        if (attributes.TryGetValue(options.UserChatId, out var value))
        {
            if (long.TryParse(value.N, out var result))
            {
                return result;
            }
            else
            {
                LogParsingError(options.UserChatId, value.N);
            }
        }
        return 0;
    }

    private void LogParsingError(string name, string value)
    {
        logger.LogWarning("Error parsing dynamoDB attribute: {Name}, value: {Value}", name, value);
    }
}


