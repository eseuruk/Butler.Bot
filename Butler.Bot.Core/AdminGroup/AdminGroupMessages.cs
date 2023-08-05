namespace Butler.Bot.Core.AdminGroup;

public class AdminGroupMessages
{
    public string ButtonApprove { get; init; } = "Принять";

    public string ButtonDecline { get; init; } = "Отклонить";

    public string ButtonDelete { get; init; } = "Удалить из группы";

    public string ReportJoinRequest { get; init; } = "Новый запрос от {0}\r\n#whois\r\n{1}";

    public string ReportUserAdded { get; init; } = "Новый член группы {0}\r\n#whois\r\n{1}";

    public string MarkJoinRequestAsApproved { get; init; } = "Принято {0}";

    public string MarkJoinRequestAsDeclined { get; init; } = "Отклонено {0}";

    public string MarkUserAsDeleted { get; init; } = "Пользователь удален {0}";
}

