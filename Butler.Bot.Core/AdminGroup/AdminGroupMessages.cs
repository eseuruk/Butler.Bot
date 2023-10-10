namespace Butler.Bot.Core.AdminGroup;

public class AdminGroupMessages
{
    public string ButtonApprove { get; init; } = "Принять";

    public string ButtonDecline { get; init; } = "Отклонить";

    public string ButtonDelete { get; init; } = "Удалить из группы";

    public string ReportJoinRequest { get; init; } = "<b>Запрос на вход в группу</b>\n{0}\n#whois\n{1}";

    public string ReportUserAdded { get; init; } = "<b>Новый член группы</b>\n{0}\n#whois\n{1}";

    public string MarkJoinRequestAsApproved { get; init; } = "Принято {0}";

    public string MarkJoinRequestAsDeclined { get; init; } = "Отклонено {0}";

    public string MarkUserAsDeleted { get; init; } = "Пользователь удален {0}";
}

