namespace Butler.Bot.Core.Tests.TargetGroup;

public class TargetGroupMentionStrategyTests
{
    private readonly TargetGroupMentionStrategy mentionStrategy;

    public TargetGroupMentionStrategyTests()
    {
        var options = new ButlerOptions
        {
            TargetGroupOptions = new TargetGroupOptions
            {
                UserNameMaxLength = 30
            }
        };

        var optionsWrapper = Options.Create(options);
        mentionStrategy = new TargetGroupMentionStrategy(optionsWrapper);
    }

    [Theory]
    [InlineData(123, "jdbxio", "John", "Doe", "@jdbxio")]
    [InlineData(262, null, "John", null, "<a href='tg://user?id=262'>John</a>")]
    [InlineData(364, null, "John", "Doe", "<a href='tg://user?id=364'>John Doe</a>")]
    [InlineData(465, null, "Aleksandrina", "Buenaventura Wetherington", "<a href='tg://user?id=465'>Aleksandrina Buenaventura Weth...</a>")]
    public void GetUserMention(long id, string? userName, string firstName, string? lastName, string expected)
    {
        // Arrange
        var user = new User { Id = id, Username = userName, FirstName = firstName, LastName = lastName };

        // Act
        var result = mentionStrategy.GetUserMention(user);

        // Assert
        result.Should().Be(expected);
    }
}