namespace Butler.Bot.AWS;

public class InstallReportDto
{
    [JsonProperty("ButlerVersion")]
    public Version ButlerVersion { get; set; } = new Version();

    [JsonProperty("Status")]
    [JsonConverter(typeof(StringEnumConverter))]
    public InstallStatus Status { get; set; } = InstallStatus.Fail;

    [JsonProperty("Entries")]
    public Dictionary<string, Entry> Entries { get; } = new Dictionary<string, Entry>();

    public class Entry
    {
        [JsonProperty("Status")]
        [JsonConverter(typeof(StringEnumConverter))]
        public InstallStatus Status { get; set; } = InstallStatus.Fail;

        [JsonProperty("Description")]
        public string Description { get; set; } = string.Empty;
    }
}

