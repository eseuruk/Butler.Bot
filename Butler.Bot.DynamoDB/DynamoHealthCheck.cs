namespace Butler.Bot.DynamoDB;

public class DynamoHealthCheck : IHealthCheck
{
    private readonly DynamoJoinRequestTable table;
    private readonly ILogger<DynamoHealthCheck> logger;

    public DynamoHealthCheck(DynamoJoinRequestTable table, ILogger<DynamoHealthCheck> logger)
    {
        this.table = table;
        this.logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        const long testID = 0L;

        try
        {
            var response = await table.GetItemAsync(testID, cancellationToken);
            
            logger.LogInformation("DynamoDB queried for test id: {Id}, response: {responseId}", testID, response?.UserId);
            return HealthCheckResult.Healthy("DynamoDB queried succesfully");
        }
        catch (Exception ex) 
        {
            logger.LogError(ex, "Error quering DynamoDB for test id: {Id}", testID);
            return HealthCheckResult.Unhealthy($"Error: {ex.Message}");
        }
    }
}
