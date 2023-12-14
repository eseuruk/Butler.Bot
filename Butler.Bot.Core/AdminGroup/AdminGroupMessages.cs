namespace Butler.Bot.Core.AdminGroup;

public class AdminGroupMessages
{
    public string ButtonApprove { get; set; } = "Принять";

    public string ButtonDecline { get; set; } = "Отклонить";

    public string ButtonDelete { get; set; } = "Удалить из группы";

    public string ButtonDeleteConfirm { get; set; } = "Подтведить удаление";

    public string ButtonCancel { get; set; } = "Отменить";

    public string ReportJoinRequest { get; set; } = "<b>Запрос на вход в группу</b>\n{0}\n#whois\n{1}";

    public string ReportUserAdded { get; set; } = "<b>Новый член группы</b>\n{0}\n#whois\n{1}";

    public string MarkJoinRequestAsApproved { get; set; } = "Принято {0}";

    public string MarkJoinRequestAsDeclined { get; set; } = "Отклонено {0}";

    public string MarkUserAsDeleted { get; set; } = "Удалено {0}";
}

