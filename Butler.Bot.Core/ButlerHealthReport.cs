namespace Butler.Bot.Core;

public class ButlerHealthReport
{
    public ButlerHealthReport(Version butlerVersion, HealthReport healthReport)
    {
        Version = butlerVersion;
        Status = healthReport.Status;
        Entries = MapEntries(healthReport.Entries);
    }

    [JsonProperty("ButlerVersion")]
    public Version Version {  get; }

    [JsonProperty("Status")]
    [JsonConverter(typeof(StringEnumConverter))]
    public HealthStatus Status { get; }
    
    [JsonProperty("Entries")]
    public IReadOnlyDictionary<string, Entry> Entries { get; }

    public class Entry
    {
        public Entry(HealthReportEntry healthReportEntry)
        {
            Status = healthReportEntry.Status;
            Description = healthReportEntry.Description;
        }

        [JsonProperty("Status")]
        [JsonConverter(typeof(StringEnumConverter))]
        public HealthStatus Status { get; }

        [JsonProperty("Description")]
        public string? Description { get; }
    }

    private IReadOnlyDictionary<string, Entry> MapEntries(IReadOnlyDictionary<string,HealthReportEntry> reportEntries)
    {
        var result = new Dictionary<string, Entry>();
        foreach (var reportEntry in reportEntries)
        {           
            result.Add(reportEntry.Key, new Entry(reportEntry.Value));
        }
        return result;
    }
}
