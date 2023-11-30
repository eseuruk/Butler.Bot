namespace Butler.Bot.DynamoDB;

public static class DynamoDBClientFactory
{
    public static IAmazonDynamoDB CreateClient(IServiceProvider serviceProvider)
    {
        var options = serviceProvider.GetRequiredService<IOptions<DynamoUserRepositoryOptions>>().Value;

        var config = CreateConfig(options);

        return new AmazonDynamoDBClient(config);
    }

    private static AmazonDynamoDBConfig CreateConfig(DynamoUserRepositoryOptions options)
    {
        var config = new AmazonDynamoDBConfig();

        if (!string.IsNullOrEmpty(options.Region))
        {
            config.RegionEndpoint = RegionEndpoint.GetBySystemName(options.Region);
        }

        return config;
    }
}
