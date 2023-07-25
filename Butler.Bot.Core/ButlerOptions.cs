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
}
