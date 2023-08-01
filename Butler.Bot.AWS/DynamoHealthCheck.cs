using Amazon.DynamoDBv2.DataModel;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Butler.Bot.AWS;

public class DynamoHealthCheck : IHealthCheck
{
    private readonly IDynamoDBContext dbContext;
    private readonly ILogger<DynamoHealthCheck> logger;

    public DynamoHealthCheck(IDynamoDBContext dbContext, ILogger<DynamoHealthCheck> logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        const long testID = 0L;

        try
        {
            var response = await dbContext.LoadAsync<DynamoJoinRequest>(testID);
            
            logger.LogInformation("DynamoDB queried for test id: {Id}, response: {responseId}", testID, response?.UserId);
            return HealthCheckResult.Healthy("DynamoDB queried succesfully");
        }
        catch (Exception ex) 
        {
            logger.LogError(ex, "Error quering DynamoDB for test id: {Id}", testID);
            return HealthCheckResult.Unhealthy("Error quering DynamoDB");
        }
    }
}
