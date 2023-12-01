namespace Butler.Bot.Sqlite;

public class SqliteHealthCheck : IHealthCheck
{
    private readonly SqliteDatabase database;
    private readonly ILogger<SqliteHealthCheck> logger;

    public SqliteHealthCheck(SqliteDatabase database, ILogger<SqliteHealthCheck> logger)
    {
        this.database = database;
        this.logger = logger;
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        if (!database.IsExist())
        {
            logger.LogInformation("Database does not exist: {DatabaseFileName}", database.FileName);
            return Task.FromResult(HealthCheckResult.Healthy("Database does not exist"));
        }

        const long testID = 0L;

        try
        {
            var request = database.SelectUserRequest(testID);
            
            logger.LogInformation("Sqlite database queried for test id: {Id}, response: {responseId}", testID, request?.UserId);
            return Task.FromResult(HealthCheckResult.Healthy("Sqlite database queried succesfully"));
        }
        catch (Exception ex) 
        {
            logger.LogError(ex, "Error quering Sqlite database for test id: {Id}", testID);
            return Task.FromResult(HealthCheckResult.Unhealthy($"Error: {ex.Message}"));
        }
    }
}
