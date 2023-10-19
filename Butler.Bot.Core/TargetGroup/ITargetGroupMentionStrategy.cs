using Telegram.Bot.Types;

namespace Butler.Bot.Core.TargetGroup;

public interface ITargetGroupMentionStrategy
{
    string GetUserMention(User user);
}