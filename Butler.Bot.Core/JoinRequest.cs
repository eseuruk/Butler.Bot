namespace Butler.Bot.Core;

public record JoinRequest
{
    public long UserId { get; init; }

    public string Whois { get; init; } = string.Empty;

    public int WhoisMessageId { get; init; } = 0;

    public long UserChatId { get; init; } = 0;

    public bool IsWhoisProvided { get { return !string.IsNullOrEmpty(Whois); } }

    public bool IsWhoisMessageWritten { get { return WhoisMessageId != 0; } }

    public bool IsUserChatIdSaved { get { return UserChatId != 0; } }
}
