using System.Net;

namespace Butler.Bot.AWS.Tests;

public class BotControllerTests : IClassFixture<ButlerApplicationFactory>
{
    private readonly ButlerApplicationFactory factory;


    public BotControllerTests(ButlerApplicationFactory factory)
    {
        this.factory = factory;
    }

    [Fact]
    public async Task BotController_Update_CanHandleMissedSecurityToken()
    {
        // Arrange
        var client = factory.CreateClient();

        // Act
        var result = await client.PostAsync("/update", new Update()
        {
            Id = 1
        }.AsBodyJson());
                
        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task BotController_Update_CanHandleAuthorizedToken()
    {
        // Arrange
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Telegram-Bot-Api-Secret-Token", "TEST_SECRET_TOCKEN");

        // Act
        var result = await client.PostAsync("/update", new Update()
        {
            Id = 10
        }.AsBodyJson());

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}