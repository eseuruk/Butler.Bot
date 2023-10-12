using Telegram.Bot.Types;

namespace Butler.Bot.Core
{
    public interface IUpdateHandler
    {
        Task<bool> TryHandleUpdateAsync(Update update, CancellationToken cancellationToken);
    }
}