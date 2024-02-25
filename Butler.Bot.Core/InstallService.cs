namespace Butler.Bot.Core;

public class InstallService : IInstallService
{
    public readonly IReadOnlyCollection<IComponentInstaller> componentInstallers;
    public readonly ILogger<InstallService> logger;
    
    public InstallService(IEnumerable<IComponentInstaller> componentInstallers, ILogger<InstallService> logger)
    {
        this.componentInstallers = componentInstallers.ToList();
        this.logger = logger;
    }

    public async Task<InstallReport> InstallAsync(BotExecutionContext context, ComponentFilter filter, CancellationToken cancellationToken)
    {
        var results = new Dictionary<string, InstallResult>();

        foreach (var installer in componentInstallers)
        {
            if (filter.AcceptComponent(installer.ComponentId))
            {
                logger.LogInformation("Installing component: {componentId}", installer.ComponentId);

                var result = await installer.InstallAsync(context, cancellationToken);
                results.Add(installer.ComponentId, result);
            }
        }

        return new InstallReport(results);
    }

    public async Task<InstallReport> UninstallAsync(BotExecutionContext context, ComponentFilter filter, CancellationToken cancellationToken)
    {
        var results = new Dictionary<string, InstallResult>();

        foreach (var installer in componentInstallers)
        {
            if (filter.AcceptComponent(installer.ComponentId))
            {
                logger.LogInformation("Uninstalling component: {componentId}", installer.ComponentId);

                var result = await installer.UninstallAsync(context, cancellationToken);
                results.Add(installer.ComponentId, result);
            }
        }

        return new InstallReport(results);
    }
}
