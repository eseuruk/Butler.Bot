using Butler.Bot.Core;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;

namespace Butler.Bot.Sqlite;

public class Database
{
    private readonly SqliteUserRepositoryOptions options;

    public Database(IOptions<SqliteUserRepositoryOptions> options)
    {
        this.options = options.Value;
    }

    public bool IsDatabaseExist()
    {
        return File.Exists(options.DatabaseFilePath);
    }

    public void CreateDatabase()
    {
        using (var connection = new SqliteConnection(options.ConnectionString))
        {
            var sql = @"CREATE TABLE JoinRequests (
                UserId INTEGER PRIMARY KEY,
                Whois TEXT,
                WhoisMessageId INTEGER,
                UserChatId INTEGER
            );";

            connection.Execute(sql);
        }
    }

    public JoinRequest? SelectUserRequest(long userId)
    {
        using (var connection = new SqliteConnection(options.ConnectionString))
        {
            var sql = @"SELECT UserId, Whois, WhoisMessageId, UserChatId
                FROM JoinRequests 
                WHERE UserId = @UserId";

            return connection.QueryFirstOrDefault<JoinRequest?>(sql, new { UserId = userId });
        }
    }

    public void InsertUserRequest(JoinRequest request)
    {
        using (var connection = new SqliteConnection(options.ConnectionString))
        {
            var sql = @"INSERT INTO JoinRequests (UserId, Whois, WhoisMessageId, UserChatId)
                VALUES (@UserId, @Whois, @WhoisMessageId, @UserChatId)";
            
            connection.Execute(sql, new { UserId = request.UserId, Whois = request.Whois, WhoisMessageId = request.WhoisMessageId, UserChatId = request.UserChatId });
        }
    }

    public void UpdateUserRequest(JoinRequest request)
    {
        using (var connection = new SqliteConnection(options.ConnectionString))
        {
            var sql = @"UPDATE JoinRequests 
                SET Whois = @Whois,
                    WhoisMessageId = @WhoisMessageId,
                    UserChatId = @UserChatId
                WHERE UserId = @UserId";

            connection.Execute(sql, new { UserId = request.UserId, Whois = request.Whois, WhoisMessageId = request.WhoisMessageId, UserChatId = request.UserChatId });
        }
    }
}
