namespace Butler.Bot.AWS.Tests;

public class TestInstaller : IComponentInstaller
{
    public TestInstaller(string componetId)
    {
        ComponentId = componetId;
    }

    public string ComponentId { get; }

    public Task<InstallResult> InstallAsync(Core.BotExecutionContext context, CancellationToken cancellationToken)
    {
        return Task.FromResult(InstallResult.Ok($"{ComponentId} installed"));
    }

    public Task<InstallResult> UninstallAsync(Core.BotExecutionContext context, CancellationToken cancellationToken)
    {
        return Task.FromResult(InstallResult.Ok($"{ComponentId} uninstalled"));
    }
}
