namespace Butler.Bot.Core;

public class HealthCheckReport
{
    public HealthCheckReport(IReadOnlyDictionary<string, HealthCheckResult> results)
    {
        (Entries, Status) = MapEntries(results);
    }

    public HealthStatus Status { get; }

    public IReadOnlyDictionary<string, Entry> Entries { get; }

    public class Entry
    {
        public Entry(HealthCheckResult result)
        {
            Status = result.Status;
            Description = result.Description;
        }

        public HealthStatus Status { get; }

        public string Description { get; }
    }

    private (IReadOnlyDictionary<string, Entry>, HealthStatus) MapEntries(IReadOnlyDictionary<string, HealthCheckResult> results)
    {
        var entries = new Dictionary<string, Entry>();
        var agregatedStatus = HealthStatus.Healthy;

        foreach (var result in results)
        {
            entries.Add(result.Key, new Entry(result.Value));

            UpdateAgregatedStatus(ref agregatedStatus, result.Value.Status);
        }

        return (entries, agregatedStatus);
    }

    private void UpdateAgregatedStatus(ref HealthStatus agregatedStatus, HealthStatus componentStatus)
    {
        if (componentStatus == HealthStatus.Unhealthy)
        {
            agregatedStatus = HealthStatus.Unhealthy;
        }
        else if (componentStatus == HealthStatus.Degraded && agregatedStatus == HealthStatus.Healthy)
        {
            agregatedStatus = HealthStatus.Degraded;
        }
    }
}

