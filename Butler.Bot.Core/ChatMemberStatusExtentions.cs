using Telegram.Bot.Types.Enums;

namespace Butler.Bot.Core;

public static class ChatMemberStatusExtentions
{
    public static bool IsAlreadyMember(this ChatMemberStatus memberStatus)
    {
        return
            memberStatus == ChatMemberStatus.Creator ||
            memberStatus == ChatMemberStatus.Administrator ||
            memberStatus == ChatMemberStatus.Member ||
            memberStatus == ChatMemberStatus.Restricted;
    }

    public static bool IsNotMember(this ChatMemberStatus memberStatus)
    {
        return
            memberStatus == ChatMemberStatus.Kicked ||
            memberStatus == ChatMemberStatus.Left;
    }


    public static bool IsRemovableMember(this ChatMemberStatus memberStatus)
    {
        return
            memberStatus == ChatMemberStatus.Administrator ||
            memberStatus == ChatMemberStatus.Member ||
            memberStatus == ChatMemberStatus.Restricted;
    }
}
