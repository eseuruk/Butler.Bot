using Butler.Bot.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Telegram.Bot.Types;

namespace Butler.Bot.AWS;

[ApiController]
public class BotController : ControllerBase
{
    private readonly ButlerBot bot;
    private readonly UpdateService updateService;
    private readonly SecretService secretService;

    private readonly ILogger<BotController> logger;

    public BotController(ButlerBot bot, UpdateService updateService, SecretService secretService, ILogger<BotController> logger)
    {
        this.bot = bot;
        this.updateService = updateService;
        this.secretService = secretService;
        this.logger = logger;
    }

    [HttpGet]
    [Route("/options")]
    public ButlerOptions ShowConfigAsync()
    {
        return bot.Options;
    }

    [HttpPost]
    [Route("/update")]
    public async Task<IActionResult> UpdateAsync([FromBody] Update update, [FromHeader(Name = "X-Telegram-Bot-Api-Secret-Token")] string? secretToken, CancellationToken cancellationToken)
    {
        if (!secretService.ValidateToken(secretToken))
        {
            return Forbid("X-Telegram-Bot-Api-Secret-Token is invalid");
        }
        
        try
        {
            await updateService.HandleUpdateAsync(update, cancellationToken);
        }
        catch (Exception ex) 
        {
            logger.LogCritical(ex, "Update WebHook failed");
        }

        return Ok();
    }
}
