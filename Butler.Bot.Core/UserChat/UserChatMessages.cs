namespace Butler.Bot.Core.UserChat;

public class UserChatMessages
{
    public string ButtonApply { get; init; } = "Приступить";

    public string ButtonContinue { get; init; } = "Продолжить";

    public string ButtonAmend { get; init; } = "Исправить";

    public string SayHello { get; init; } = "Привет! Я помогу тебя зайти в группу {0}. Если не против, я задам тебе несколько вопросов, перед тем как ты сможешь туда попасть.";

    public string SayConfused { get; init; } = "Наверное ты хочешь зайти в группу {0}. Если да, то нажми на кнопку ниже чтоб продолжить.";

    public string AskForWhois { get; init; } = "Пожалуйста, представься как тебя зовут, чем занимаешься и увлекаешься, что тебя привело в этот чат ({0} символов)";

    public string SayWhoisTooShort { get; init; } = "Хм. В сообщении {0}, а надо минимум {1} символов. Пожалуйста, попробуй еще раз и добавь больше информации про себя.";

    public string SayWhoisOkAndAskToRequestAccess { get; init; } = "Спасибо за детальное описание. Теперь, нажми на кнопку ниже и запроси доступ к группе.";

    public string SayUsedToBeMember { get; init; } = "Похоже, ты состоял в групе раньше но вышел из нее. Пожалуйста, нажми на кнопку ниже и запроси доступ снова.";

    public string SayAlreadyMember { get; init; } = "Ты уже состоишь в нашей группе. Нажми на кнопну ниже, чтобы перейти в нее.";

    public string SayRequestApproved { get; init; } = "Запрос в группу был одобрен. Нажми на кнопку ниже чтобы перейти в нее.";

    public string SayRequestDeclined { get; init; } = "К сожалению, запрос в группу был отклонен. Попробуй исправить описание и запросить снова.";

    public string SayUserDeleted { get; init; } = "К сожалению, администраторы удалили тебя из группы из-за плохого описания. Попробуй исправить его и запросить доступ снова.";
}

