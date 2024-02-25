namespace Butler.Bot.Core;

public interface IComponentInstaller
{
    string ComponentId { get; }

    Task<InstallResult> InstallAsync(BotExecutionContext context, CancellationToken cancellationToken);

    Task<InstallResult> UninstallAsync(BotExecutionContext context, CancellationToken cancellationToken);
}
