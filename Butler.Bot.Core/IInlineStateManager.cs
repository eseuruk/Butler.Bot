using Telegram.Bot.Types;

namespace Butler.Bot.Core;

public interface IInlineStateManager
{
    T GetStateFromMessage<T>(Message message);

    string InjectStateIntoMessageHtml<T>(string messageHtml, T state);
}