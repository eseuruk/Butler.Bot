namespace Butler.Bot.Core.AdminGroup;

public class AdminGroupMentionStrategy : IAdminGroupMentionStrategy
{
    private readonly AdminGroupOptions options;

    public AdminGroupMentionStrategy(IOptions<ButlerOptions> options)
    {
        this.options = options.Value.AdminGroupOptions;
    }

    public string GetAdminMention(User user)
    {
        var mention = user.GetUserNameRefIfExist();
        if (mention == null)
        {
            mention = user.GetFullNameText(options.AdminNameMaxLength);
        }
        return mention;
    }

    public string GetUserMention(User user)
    {
        var mention = user.GetFullNameText(options.UserNameMaxLength);

        var userName = user.GetUserNameRefIfExist();
        if (userName != null)
        {
            mention += " " + userName;
        }

        return mention;
    }
}

