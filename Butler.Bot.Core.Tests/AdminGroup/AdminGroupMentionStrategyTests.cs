namespace Butler.Bot.Core.Tests.AdminGroup;

public class AdminGroupMentionStrategyTests
{
    private readonly AdminGroupMentionStrategy mentionStrategy;

    public AdminGroupMentionStrategyTests()
    {
        var options = new ButlerOptions
        {
            AdminGroupOptions = new AdminGroupOptions
            {
                UserNameMaxLength = 100,
                AdminNameMaxLength = 30,
            }
        };

        var optionsWrapper = Options.Create(options);
        mentionStrategy = new AdminGroupMentionStrategy(optionsWrapper);
    }

    [Theory]
    [InlineData("jdbxio", "John", "Doe", "@jdbxio")]
    [InlineData(null, "John", null, "John")]
    [InlineData(null, "John", "Doe", "John Doe")]
    [InlineData(null, "Aleksandrina", "Buenaventura Wetherington", "Aleksandrina Buenaventura Weth...")]
    [InlineData(null, "Flag emoji", "at the end ------>\ud83c\udff4\udb40\udc67\udb40\udc62\udb40\udc65\udb40\udc6e\udb40\udc67\udb40\udc7f", "Flag emoji at the end ------>\ud83c\udff4\udb40\udc67\udb40\udc62\udb40\udc65\udb40\udc6e\udb40\udc67\udb40\udc7f")]
    public void GetAdminMention(string userName, string firstName, string lastName, string expected)
    {
        // Arrange
        var user = new User { Username = userName, FirstName = firstName, LastName = lastName };

        // Act
        var result = mentionStrategy.GetAdminMention(user);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("jdbxio", "John", "Doe", "John Doe @jdbxio")]
    [InlineData(null, "John", null, "John")]
    [InlineData(null, "John", "Doe", "John Doe")]
    [InlineData("HollyFlower", "Aleksandrina", "Buenaventura Wetherington", "Aleksandrina Buenaventura Wetherington @HollyFlower")]
    public void GetUserMention(string userName, string firstName, string lastName, string expected)
    {
        // Arrange
        var user = new User { Username = userName, FirstName = firstName, LastName = lastName };

        // Act
        var result = mentionStrategy.GetUserMention(user);

        // Assert
        result.Should().Be(expected);
    }
}