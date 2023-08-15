namespace Butler.Bot.Sqlite;

public class SqliteUserRepositoryOptions
{
    public string DatabaseFilePath { get; init; } = "./Butler.Bot.Sqlite.db";

    public string ConnectionString => $"Data Source={DatabaseFilePath}";
}

