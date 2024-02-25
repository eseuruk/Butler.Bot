namespace Butler.Bot.Core;

public class InstallReport
{
    public InstallReport(IReadOnlyDictionary<string, InstallResult> results)
    {
        (Entries, Status) = MapEntries(results);
    }

    public InstallStatus Status { get; }

    public IReadOnlyDictionary<string, Entry> Entries { get; }

    public class Entry
    {
        public Entry(InstallResult installResult)
        {
            Status = installResult.Status;
            Description = installResult.Description;
        }

        public InstallStatus Status { get; }

        public string Description { get; }
    }

    private (IReadOnlyDictionary<string, Entry>, InstallStatus) MapEntries(IReadOnlyDictionary<string, InstallResult> results)
    {
        var entries = new Dictionary<string, Entry>();
        var agregatedStatus = InstallStatus.Ok;

        foreach (var result in results)
        {
            entries.Add(result.Key, new Entry(result.Value));

            UpdateAgregatedStatus(ref agregatedStatus, result.Value.Status);
        }

        return (entries, agregatedStatus);
    }

    private void UpdateAgregatedStatus(ref InstallStatus agregatedStatus, InstallStatus componentStatus)
    {
        if (componentStatus == InstallStatus.Fail)
        {
            agregatedStatus = InstallStatus.Fail;
        }
    }
}

