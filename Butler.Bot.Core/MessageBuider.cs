namespace Butler.Bot.Core;

public static class MessageBuider
{
    public static string SafeFormat(this string format, params object?[] args)
    {
        try
        {
            return string.Format(format, args);
        }
        catch(FormatException)
        {
            return format;
        }
    }
}
