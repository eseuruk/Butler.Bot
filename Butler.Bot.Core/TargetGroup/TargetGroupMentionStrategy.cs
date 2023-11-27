namespace Butler.Bot.Core.TargetGroup;

public class TargetGroupMentionStrategy : ITargetGroupMentionStrategy
{
    private readonly TargetGroupOptions options;

    public TargetGroupMentionStrategy(IOptions<ButlerOptions> options)
    {
        this.options = options.Value.TargetGroupOptions;
    }

    public string GetUserMention(User user)
    {
        var mention = user.GetUserNameRefIfExist();
        if (mention == null)
        {
            mention = user.GetFullNameLink(options.UserNameMaxLength);
        }

        return mention;
    }
}

