using Butler.Bot.Core.AdminGroup;
using Butler.Bot.Core.TargetGroup;
using Butler.Bot.Core.UserChat;

namespace Butler.Bot.Core;

public class ButlerOptions
{
    public string TargetGroupDisplayName { get; set; } = string.Empty;

    public long TargetGroupId { get; set; } = 0;

    public long AdminGroupId { get; set; } = 0;

    public string InvitationLink { get; set; } = string.Empty;

    public string InvitationLinkName { get; set; } = string.Empty;

    public int MinWoisLength { get; set; } = 120;

    public WhoisReviewMode WhoisReviewMode { get; set; } = WhoisReviewMode.None;

    public string SiteUrl { get; set; } = "https://github.com/eseuruk/Butler.Bot";

    public AdminGroupOptions AdminGroupOptions { get; set; } = new AdminGroupOptions();

    public TargetGroupOptions TargetGroupOptions { get; set; } = new TargetGroupOptions();

    public UserChatMessages UserChatMessages { get; set; } =  new UserChatMessages();

    public TargetGroupMessages TargetGroupMessages { get; set; } = new TargetGroupMessages();

    public AdminGroupMessages AdminGroupMessages { get; set; } = new AdminGroupMessages();
}
