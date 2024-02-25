namespace Butler.Bot.AWS;

public class HealthCheckReportDto
{
    [JsonProperty("ButlerVersion")]
    public Version ButlerVersion { get; set; } = new Version();

    [JsonProperty("Status")]
    [JsonConverter(typeof(StringEnumConverter))]
    public HealthStatus Status { get; set; } = HealthStatus.Unhealthy;

    [JsonProperty("Entries")]
    public Dictionary<string, Entry> Entries { get; } = new Dictionary<string, Entry>();

    public class Entry
    {
        [JsonProperty("Status")]
        [JsonConverter(typeof(StringEnumConverter))]
        public HealthStatus Status { get; set; } = HealthStatus.Unhealthy;

        [JsonProperty("Description")]
        public string Description { get; set; } = string.Empty;
    }
}

