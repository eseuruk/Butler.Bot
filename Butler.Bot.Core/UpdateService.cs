using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;

namespace Butler.Bot.Core;

public class UpdateService : IUpdateService
{
    private readonly IEnumerable<IUpdateHandler> handlers;
    private readonly ILogger<UpdateService> logger;

    public UpdateService(IEnumerable<IUpdateHandler> handlers, ILogger<UpdateService> logger)
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
