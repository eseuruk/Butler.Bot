namespace Butler.Bot.Core.AdminGroup;

public interface IAdminGroupMentionStrategy
{
    string GetAdminMention(User user);

    string GetUserMention(User user);
}