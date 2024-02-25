using System.Net;

namespace Butler.Bot.AWS.Tests;

public class InstallControllerTests : IClassFixture<ButlerApplicationFactory>
{
    private readonly ButlerApplicationFactory factory;

    public InstallControllerTests(ButlerApplicationFactory factory)
    {
        this.factory = factory;
    }

    [Fact]
    public async Task InstallController_Install_All()
    {
        // Arrange
        var client = factory.CreateClient();

        // Act
        var result = await client.GetAsync("/install");

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);

        var report = await result.Content.ReadAsJsonAsync<InstallReportDto>();

        report.Status.Should().Be(InstallStatus.Ok);
        report.Entries.Count.Should().Be(2);
    }

    [Fact]
    public async Task InstallController_Install_Component()
    {
        // Arrange
        var client = factory.CreateClient();

        // Act
        var result = await client.GetAsync("/install?componentFilter=Component3");

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);

        var report = await result.Content.ReadAsJsonAsync<InstallReportDto>();
        report.Entries.Count.Should().Be(1);
    }

    [Fact]
    public async Task InstallController_Uninstall_All()
    {
        // Arrange
        var client = factory.CreateClient();

        // Act
        var result = await client.GetAsync("/uninstall");

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);

        var report = await result.Content.ReadAsJsonAsync<InstallReportDto>();
        report.Entries.Count.Should().Be(2);
    }

    [Fact]
    public async Task InstallController_Uninstall_Component()
    {
        // Arrange
        var client = factory.CreateClient();

        // Act
        var result = await client.GetAsync("/uninstall?componentFilter=Component3");

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);

        var report = await result.Content.ReadAsJsonAsync<InstallReportDto>();

        report.Status.Should().Be(InstallStatus.Ok);
        report.Entries.Count.Should().Be(1);
    }
}
