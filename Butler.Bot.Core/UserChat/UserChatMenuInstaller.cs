namespace Butler.Bot.Core.UserChat;

public class UserChatMenuInstaller : IComponentInstaller, IComponentHealthCheck
{
    private readonly ITelegramBotClient apiClient;
    private readonly ILogger<UserChatMenuInstaller> logger;

    private readonly BotCommand[] commands =
    {
        new BotCommand { Command = "start", Description = "join the group" },
        new BotCommand { Command = "leave", Description = "leave the group" },
        new BotCommand { Command = "version", Description = "bot version" }
    };

    public UserChatMenuInstaller(ITelegramBotClient apiClient, ILogger<UserChatMenuInstaller> logger)
    {
        this.apiClient = apiClient;
        this.logger = logger;
    }

    public string ComponentId => "UserChartMenu";

    public async Task<HealthCheckResult> CheckHealthAsync(BotExecutionContext context, CancellationToken cancellationToken)
    {
        try
        {
            var currentCommands = await apiClient.GetMyCommandsAsync(
                scope: BotCommandScope.AllPrivateChats(),
                cancellationToken: cancellationToken);

            var currentMenuDescription = CreateMenuDescription(currentCommands);
            var expectedMenuDescription = CreateMenuDescription(commands);

            if (currentMenuDescription == expectedMenuDescription)
            {
                logger.LogInformation("User chat menu: {currentMenu}", currentMenuDescription);
                return HealthCheckResult.Healthy(currentMenuDescription);
            }
            else
            {
                logger.LogWarning("User chat menu: {currentMenu}, expected: {expectedMenu}", currentMenuDescription, expectedMenuDescription);
                return HealthCheckResult.Degraded($"Menu does not match: {currentMenuDescription} expected: {expectedMenuDescription}");
            }
        }
        catch (ApiRequestException ex)
        {
            logger.LogError(ex, "Error getting user chat menu");
            return HealthCheckResult.Unhealthy($"Api error: {ex.ErrorCode} - {ex.Message}");
        }
    }

    public async Task<InstallResult> InstallAsync(BotExecutionContext context, CancellationToken cancellationToken)
    {
        try
        {
            await apiClient.SetMyCommandsAsync(
                commands: commands,
                scope: BotCommandScope.AllPrivateChats(),
                cancellationToken: cancellationToken);

            var menuDescription = CreateMenuDescription(commands);
            logger.LogInformation("User chat menu installed: {menu}", menuDescription);

            return InstallResult.Ok(menuDescription);
        }
        catch (ApiRequestException ex)
        {
            logger.LogError(ex, "Error installing user chat menu");
            return InstallResult.Fail($"Api error: {ex.ErrorCode} - {ex.Message}");
        }
    }

    public async Task<InstallResult> UninstallAsync(BotExecutionContext context, CancellationToken cancellationToken)
    {
        try
        {
            await apiClient.DeleteMyCommandsAsync(
                scope: BotCommandScope.AllPrivateChats(),
                cancellationToken: cancellationToken);

            logger.LogInformation("User chat menu removed");
            return InstallResult.Ok("Menu removed");
        }
        catch (ApiRequestException ex)
        {
            logger.LogError(ex, "Error uninstalling user chat menu");
            return InstallResult.Fail($"Api error: {ex.ErrorCode} - {ex.Message}");
        }
    }

    private string CreateMenuDescription(BotCommand[] commands)
    {
        return string.Join( '/', commands.Select(c => c.Command));
    }
}
