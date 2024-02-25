using System.Net;

namespace Butler.Bot.AWS.Tests;

public class HealthCheckControllerTests : IClassFixture<ButlerApplicationFactory>
{
    private readonly ButlerApplicationFactory factory;

    public HealthCheckControllerTests(ButlerApplicationFactory factory)
    {
        this.factory = factory;
    }

    [Fact]
    public async Task HealthCheckController_CheckHealth_All()
    {
        // Arrange
        var client = factory.CreateClient();

        // Act
        var result = await client.GetAsync("/health");

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);

        var report = await result.Content.ReadAsJsonAsync<HealthCheckReportDto>();

        report.Status.Should().Be(HealthStatus.Healthy);
        report.Entries.Count.Should().Be(2);
    }

    [Fact]
    public async Task HealthCheckController_CheckHealth_Component()
    {
        // Arrange
        var client = factory.CreateClient();

        // Act
        var result = await client.GetAsync("/health?componentFilter=Component1");

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);

        var report = await result.Content.ReadAsJsonAsync<HealthCheckReportDto>();

        report.Status.Should().Be(HealthStatus.Healthy);
        report.Entries.Count.Should().Be(1);
    }
}
