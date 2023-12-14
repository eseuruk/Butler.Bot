namespace Butler.Bot.Core.UserChat;

public class UserChatMessages
{
    public string ButtonApply { get; set; } = "Приступить";

    public string ButtonContinue { get; set; } = "Продолжить";

    public string ButtonAmend { get; set; } = "Исправить";

    public string ButtonRequestToJoin{ get; set; } = "Запросить доступ";

    public string ButtonDeleteWhois { get; set; } = "Удалить описание";

    public string ButtonLeave { get; set; } = "Покинуть группу";

    public string ButtonCancel { get; set; } = "Отменить";

    public string ShowBotVersion { get; set; } = "<b>{0}</b>\nверсия: {1}\nсайт: {2}";

    public string SayHello { get; set; } = "Привет! Я помогу тебя зайти в группу {0}. Если не против, я задам тебе несколько вопросов, перед тем как ты сможешь туда попасть.";

    public string SayConfused { get; set; } = "Наверное ты хочешь зайти в группу {0}. Если да, то нажми на кнопку ниже чтоб продолжить.";

    public string AskForWhois { get; set; } = "Пожалуйста, представься как тебя зовут, чем занимаешься и увлекаешься, что тебя привело в этот чат (минимум {0} символов)";

    public string SayWhoisTooShort { get; set; } = "Хм. В сообщении {0}, а надо минимум {1} символов. Пожалуйста, попробуй еще раз и добавь больше информации про себя.";

    public string SayWhoisOkAndAskToRequestAccess { get; set; } = "Спасибо за детальное описание. Теперь, нажми на кнопку ниже и запроси доступ к группе.";

    public string SayUsedToBeMember { get; set; } = "Похоже, ты состоял в групе раньше но вышел из нее. Пожалуйста, запроси доступ снова, либо удали старое описание чтобы написать новое.";

    public string SayAlreadyMember { get; set; } = "Ты уже состоишь в нашей группе. Нажми на кнопну ниже, чтобы перейти в нее.";

    public string SayBlockedMember { get; set; } = "Ты был удален из группы и только администраторы могут вернуть тебе доступ.";

    public string AskForForLeavingConfirmation { get; set; } = "Ты действительно хочешь покинуть группу {0} и удалить свое описание?\n\nЯ не могу удалить твои сообщения. Если хочешь удалить их, то перейди в группу и сделай это сам.";

    public string SayLeavingRequestCanceled { get; set; } = "Запрос на удаление из группы {0} был отменен.";

    public string SayUserLeft { get; set; } = "Если ты состоял в группе {0}, то ты из нее удален. Также удалено твое описание.\n\nПриходи ещё.";

    public string SayRequestApproved { get; set; } = "Запрос в группу был одобрен. Нажми на кнопку ниже чтобы перейти в нее.";

    public string SayRequestDeclined { get; set; } = "К сожалению, запрос в группу был отклонен. Попробуй исправить описание и запросить снова.";

    public string SayUserDeleted { get; set; } = "К сожалению, администраторы удалили тебя из группы из-за плохого описания. Попробуй исправить его и запросить доступ снова.";
}

