﻿namespace Butler.Bot.AWS;

[ApiController]
public class BotController : ControllerBase
{
    private readonly IUpdateService updateService;
    private readonly SecretService secretService;
    private readonly HealthCheckService healthService;

    private readonly ILogger<BotController> logger;

    public BotController(IUpdateService updateService, SecretService secretService, HealthCheckService healthService, ILogger<BotController> logger)
    {
        this.updateService = updateService;
        this.secretService = secretService;
        this.healthService = healthService;
        this.logger = logger;
    }

    [HttpGet]
    [Route("/health")]
    public async Task<ButlerHealthReport> HehthCheckAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Checking system health");

        var buttlerVersion = ButlerVersion.GetCurrent();
        var healthReport = await healthService.CheckHealthAsync(cancellationToken);

        logger.LogInformation("System health status: {Status}, buttlerVersion: {ButtlerVersion}", healthReport.Status, buttlerVersion);

        return new ButlerHealthReport(buttlerVersion, healthReport);
    }

    [HttpPost]
    [Route("/update")]
    public async Task<IActionResult> UpdateAsync([FromBody] Update update, [FromHeader(Name = "X-Telegram-Bot-Api-Secret-Token")] string? secretToken, CancellationToken cancellationToken)
    {
        if (!secretService.ValidateToken(secretToken))
        {
            return StatusCode(StatusCodes.Status403Forbidden);
        }
        
        try
        {
            logger.LogInformation("WebHook processing started");

            await updateService.HandleUpdateAsync(update, cancellationToken);

            logger.LogInformation("WebHook processing finished");
        }
        catch (Exception ex) 
        {
            logger.LogCritical(ex, "WebHook processing failed");
        }

        // always return 200 to not retry updates from telegram
        return Ok();
    }
}
