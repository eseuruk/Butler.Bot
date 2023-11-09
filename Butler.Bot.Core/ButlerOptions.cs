using Butler.Bot.Core.AdminGroup;
using Butler.Bot.Core.TargetGroup;
using Butler.Bot.Core.UserChat;

namespace Butler.Bot.Core;

public class ButlerOptions
{
    public string TargetGroupDisplayName { get; init; } = string.Empty;

    public long TargetGroupId { get; init; } = 0;

    public long AdminGroupId { get; init; } = 0;

    public string InvitationLink { get; init; } = string.Empty;

    public string InvitationLinkName { get; init; } = string.Empty;

    public int MinWoisLength { get; init; } = 120;

    public WhoisReviewMode WhoisReviewMode { get; init; } = WhoisReviewMode.None;

    public string SiteUrl { get; init; } = "https://github.com/eseuruk/Butler.Bot";

    public AdminGroupOptions AdminGroupOptions { get; init; } = new AdminGroupOptions();

    public TargetGroupOptions TargetGroupOptions { get; init; } = new TargetGroupOptions();

    public UserChatMessages UserChatMessages { get; init; } =  new UserChatMessages();

    public TargetGroupMessages TargetGroupMessages { get; init; } = new TargetGroupMessages();

    public AdminGroupMessages AdminGroupMessages { get; init; } = new AdminGroupMessages();
}
