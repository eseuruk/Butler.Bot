namespace Butler.Bot.AWS.Tests;

public class BotControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public BotControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task BotController_Update_CanHandleMissedSecurityToken()
    {
        // Arrange
        var client = _factory.CreateAnonymousClient();

        // Act
        var result = await client.PostAsync("/update", new Update()
        {
            Id = 1
        }.AsBodyJson());
        
        // Assert
        result.ShouldHave403Code();
    }
    
    [Fact]
    public async Task BotController_Update_CanHandleAuthorizedToken()
    {
        // Arrange
        int updateId = 10;
        var service = new Mock<IUpdateService>();
        service.Setup(_ => _.HandleUpdateAsync(It.IsAny<Update>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        var client =
            _factory.CreateAuthorizedClient("TEST_TOKEN", services => services.AddSingleton(_ => service.Object));

        // Act
        var result = await client.PostAsync("/update", new Update()
        {
            Id = updateId
        }.AsBodyJson());
        
        // Assert
        result.ShouldHave200Code();
        service.Verify(_ => _.HandleUpdateAsync(It.Is<Update>(a => a.Id == updateId), It.IsAny<CancellationToken>()),
            Times.Once);
    }
    
    [Fact]
    public async Task BotController_Update_HandlerCalled()
    {
        // Arrange
        int updateId = 10;
        var handler = new Mock<IUpdateHandler>();
        handler.Setup(_ => _.TryHandleUpdateAsync(It.IsAny<Update>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(true));
        var client =
            _factory.CreateAuthorizedClient("TEST_TOKEN", services => services.AddSingleton(_ => handler.Object));

        // Act
        var result = await client.PostAsync("/update", new Update()
        {
            Id = updateId
        }.AsBodyJson());
        
        // Assert
        result.ShouldHave200Code();
        handler.Verify(_ => _.TryHandleUpdateAsync(It.Is<Update>(a => a.Id == updateId), It.IsAny<CancellationToken>()),
            Times.Once);
    }
    
}