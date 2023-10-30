using Newtonsoft.Json;

namespace Butler.Bot.Core.AdminGroup;

public class PostJoinInlineState
{
    public const int CurrentVersion = 1;

    [JsonProperty("v")]
    public int Version { get; set; } = CurrentVersion;

    [JsonProperty("wmi")]
    public int WhoisMessageId { get; set; }

    [JsonProperty("uid")]
    public long UserId { get; set; }
}
