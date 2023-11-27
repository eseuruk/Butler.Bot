namespace Butler.Bot.Core.Tests;

public class ButlerVersionTests
{
    [Fact]
    public void GetCurrent_ShouldReturnThreePartVersionNumber()
    {
        // act
        var version = ButlerVersion.GetCurrent();

        // assert
        version.ToString().Should().MatchRegex(@"^(0|[1-9]\d*)\.(0|[1-9]\d*)\.(0|[1-9]\d*)$", "version should have three parts only");
    }
}
