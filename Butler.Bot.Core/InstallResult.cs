namespace Butler.Bot.Core;

public enum InstallStatus
{
    Fail,
    Ok
}

public class InstallResult
{
    public InstallResult(InstallStatus status, string description)
    {
        Status = status;
        Description = description;
    }

    public InstallStatus Status { get; }

    public string Description { get; }

    public static InstallResult Ok(string description)
    {
        return new InstallResult(InstallStatus.Ok, description);
    }

    public static InstallResult Fail(string description)
    {
        return new InstallResult(InstallStatus.Fail, description);
    }
}
