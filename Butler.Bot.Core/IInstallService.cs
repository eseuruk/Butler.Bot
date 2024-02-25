namespace Butler.Bot.Core;

public interface IInstallService
{
    Task<InstallReport> InstallAsync(BotExecutionContext context, ComponentFilter filter, CancellationToken cancellationToken);

    Task<InstallReport> UninstallAsync(BotExecutionContext context, ComponentFilter filter, CancellationToken cancellationToken);
}
