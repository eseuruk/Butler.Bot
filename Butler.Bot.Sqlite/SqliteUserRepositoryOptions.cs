namespace Butler.Bot.Sqlite;

public class SqliteUserRepositoryOptions
{
    public string DatabaseFilePath { get; init; } = "./Buttler.Bot.Sqlite.db";

    public string ConnectionString => $"Data Source={DatabaseFilePath}";
}

