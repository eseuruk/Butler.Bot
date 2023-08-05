namespace Butler.Bot.Core.TargetGroup;

public class TargetGroupMessages
{
    public string SayHelloToNewMember { get; init; } = "Приветствуем {0}\r\n#whois\r\n{1}";

    public string SayHelloToUnknownNewMember { get; init; } = "Приветствуем {0}";

    public string SayHelloAgain { get; init; } = "С возвращением {0}";
}

