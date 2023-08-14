using Telegram.Bot.Types;

namespace Butler.Bot.Core;

public static class MentionBuider
{ 
    public static string GetMention(User user)
    {
        var displayName = user.FirstName;
        if (!string.IsNullOrEmpty(user.LastName))
        {
            displayName += " " + user.LastName;
        }

        return $"<a href='tg://user?id={user.Id}'>{displayName}</a>";
    }
}

