using Newtonsoft.Json;

namespace Butler.Bot.Core.AdminGroup;

public class PreJoinInlineState
{
    public const int CurrentVersion = 1;

    [JsonProperty("v")]
    public int Version { get; set; } = CurrentVersion;

    [JsonProperty("uid")]
    public long UserId { get; set; }
}
