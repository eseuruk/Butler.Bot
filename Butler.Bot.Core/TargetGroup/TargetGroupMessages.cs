namespace Butler.Bot.Core.TargetGroup;

public class TargetGroupMessages
{
    public string SayHelloToNewMember { get; set; } = "Приветствуем {0}\r\n#whois\r\n{1}";

    public string SayHelloToUnknownNewMember { get; set; } = "Приветствуем {0}";

    public string SayHelloAgain { get; set; } = "С возвращением {0}";

    public string SayLeavingToChangeWhois{ get; set; } = "{0} покидает чат исправлять описание";

    public string SayLeaving { get; set; } = "{0} покидает чат";
}

