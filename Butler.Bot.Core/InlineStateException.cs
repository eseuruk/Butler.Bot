namespace Butler.Bot.Core;

public class InlineStateException : Exception
{
    protected InlineStateException()
          : base()
    { }

    public InlineStateException(string message)
       : base(message)
    { }

    public InlineStateException(string message, Exception innerException)
      :  base(message, innerException)
    { }
}

