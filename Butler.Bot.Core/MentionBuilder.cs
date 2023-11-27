using System.Globalization;

namespace Butler.Bot.Core;

public static class MentionBuilder
{
    public static string? GetUserNameRefIfExist(this User user)
    {
        if (!string.IsNullOrEmpty(user.Username))
        {
            return "@" + user.Username;
        }

        return null;
    }

    public static string GetFullNameLink(this User user, int maxLength)
    {
        var displayName = GetFullNameText(user, maxLength);
        return $"<a href='tg://user?id={user.Id}'>{displayName}</a>";
    }

    public static string GetFullNameText(this User user, int maxLength)
    {
        var fullName = user.FirstName;
        if (!string.IsNullOrEmpty(user.LastName))
        {
            fullName += " " + user.LastName;
        }

        return TrancateName(fullName, maxLength);
    }

    private static string TrancateName(string name, int limit)
    {
        if (limit <= 0) return name;

        var stringInfo = new StringInfo(name);
        if (stringInfo.LengthInTextElements > limit)
        {
            return stringInfo.SubstringByTextElements(0, limit) + "...";
        }

        return name;
    }
}

