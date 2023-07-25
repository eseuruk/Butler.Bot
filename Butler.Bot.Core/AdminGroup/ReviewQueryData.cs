namespace Butler.Bot.Core.AdminGroup;

public class ReviewQueryData
{
    public string Operation { get; init; } = string.Empty;

    public long UserId { get; init; }

    public override string ToString()
    {
        return ToString(Operation, UserId);
    }

    public static string ToString(string operation, long userId)
    {
        return $"{operation}:{userId}";
    }

    public static bool TryParse(string? dataString, out ReviewQueryData data)
    {
        if (dataString != null)
        {
            var parts = dataString.Split(":");
            if (parts.Length == 2)
            {
                if (long.TryParse(parts[1], out long userId))
                {
                    data = new ReviewQueryData { Operation = parts[0], UserId = userId };
                    return true;
                }
            }
        }

        data = new ReviewQueryData();
        return false;
    }
}

