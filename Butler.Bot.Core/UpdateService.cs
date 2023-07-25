using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;

namespace Butler.Bot.Core;

public class UpdateService
{
    private readonly IEnumerable<UpdateHandlerBase> handlers;
    private readonly ILogger<UpdateService> logger;

    public UpdateService(IEnumerable<UpdateHandlerBase> handlers, ILogger<UpdateService> logger)
    {
        this.handlers = handlers;
        this.logger = logger;
    }

    public async Task HandleUpdateAsync(Update update, CancellationToken cancellationToken)
    {
        foreach (var handler in handlers)
        {
            var handled = await handler.TryHandleUpdateAsync(update, cancellationToken);
            if (handled) return;
        }
    }
}
