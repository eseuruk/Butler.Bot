namespace Butler.Bot.Core;

public interface IWhoisValidator
{
    (bool, string) CheckMessageText(string messageText);
}


