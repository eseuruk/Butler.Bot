using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;

namespace Butler.Bot.Core.Tests;

public class InlineStateManagerTests
{
    private readonly InlineStateManager inlineStateManager;

    public InlineStateManagerTests()
    {
        inlineStateManager = new InlineStateManager();
    }

    [Fact]
    public void InjectStateIntoMessageHtml_EN_ShouldReturnMessageWithStateLink()
    {
        // Arrange
        var state = new User{ Id = 100, FirstName = "Johhn", LastName = "Smith", Username = "UsernameGenius77" };
        var messageHtml = "<p>Hello, world!</p>";

        // Act
        var resultHtml = inlineStateManager.InjectStateIntoMessageHtml(messageHtml, state);

        // Assert
        resultHtml.Should().Be("<a href='tg://btn/eyJpZCI6MTAwLCJpc19ib3QiOmZhbHNlLCJmaXJzdF9uYW1lIjoiSm9oaG4iLCJsYXN0X25hbWUiOiJTbWl0aCIsInVzZXJuYW1lIjoiVXNlcm5hbWVHZW5pdXM3NyJ9'>\u200b</a><p>Hello, world!</p>");
    }

    [Fact]
    public void InjectStateIntoMessageHtml_RU_ShouldReturnMessageWithStateLink()
    {
        // Arrange
        var state = new User { Id = 100, FirstName = "Джон", LastName = "Смит", Username = "UsernameGenius77" };
        var messageHtml = "<p>Hello, world!</p>";

        // Act
        var resultHtml = inlineStateManager.InjectStateIntoMessageHtml(messageHtml, state);

        // Assert
        resultHtml.Should().Be("<a href='tg://btn/eyJpZCI6MTAwLCJpc19ib3QiOmZhbHNlLCJmaXJzdF9uYW1lIjoi0JTQttC+0L0iLCJsYXN0X25hbWUiOiLQodC80LjRgiIsInVzZXJuYW1lIjoiVXNlcm5hbWVHZW5pdXM3NyJ9'>​</a><p>Hello, world!</p>");
    }

    [Fact]
    public void GetStateFromMessage_EN_ShouldReturnDeserializedState()
    {
        // Arrange
        var message = CreateMessageWithTextLink("tg://btn/eyJpZCI6MTAwLCJpc19ib3QiOmZhbHNlLCJmaXJzdF9uYW1lIjoiSm9oaG4iLCJsYXN0X25hbWUiOiJTbWl0aCIsInVzZXJuYW1lIjoiVXNlcm5hbWVHZW5pdXM3NyJ9");

        // Act
        var resultState = inlineStateManager.GetStateFromMessage<User>(message);

        // Assert
        var state = new User { Id = 100, FirstName = "Johhn", LastName = "Smith", Username = "UsernameGenius77" };
        resultState.Should().BeEquivalentTo(state);
    }

    [Fact]
    public void GetStateFromMessage_RU_ShouldReturnDeserializedState()
    {
        // Arrange
        var message = CreateMessageWithTextLink("tg://btn/eyJpZCI6MTAwLCJpc19ib3QiOmZhbHNlLCJmaXJzdF9uYW1lIjoi0JTQttC+0L0iLCJsYXN0X25hbWUiOiLQodC80LjRgiIsInVzZXJuYW1lIjoiVXNlcm5hbWVHZW5pdXM3NyJ9");

        // Act
        var resultState = inlineStateManager.GetStateFromMessage<User>(message);

        // Assert
        var state = new User { Id = 100, FirstName = "Джон", LastName = "Смит", Username = "UsernameGenius77" };
        resultState.Should().BeEquivalentTo(state);
    }

    [Fact]
    public void GetStateFromMessage_ShouldThrowInlineStateExceptionIfStateLinkNotFound()
    {
        // Arrange
        var message = CreateMessageWithTextLink("https://example.com");

        // Act
        var act = () => inlineStateManager.GetStateFromMessage<User>(message);

        // Assert
        act.Should().Throw<InlineStateException>().WithMessage("Inline state link is not found in the message*");
    }

    [Fact]
    public void GetStateFromMessage_ShouldThrowInlineStateExceptionOnInvalidBase64Data()
    {
        // Arrange
        var message = CreateMessageWithTextLink($"tg://btn/InvalidBase64Data");

        // Act
        var act = () => inlineStateManager.GetStateFromMessage<User>(message);

        // Assert
        act.Should().Throw<InlineStateException>().WithMessage("Error parsing base64*");
    }

    [Fact]
    public void GetStateFromMessage_ShouldThrowInlineStateExceptionOnInvalidJson()
    {
        // Arrange
        var message = CreateMessageWithTextLink($"tg://btn/SW52YWxpZEpzb24=");

        // Act
        var act = () => inlineStateManager.GetStateFromMessage<User>(message);

        // Assert
        act.Should().Throw<InlineStateException>().WithMessage("Error deserializing state: InvalidJson");
    }

    [Fact]
    public void GetStateFromMessage_ShouldThrowInlineStateExceptionOnEmptyJson()
    {
        // Arrange
        var message = CreateMessageWithTextLink($"tg://btn/");

        // Act
        var act = () => inlineStateManager.GetStateFromMessage<User>(message);

        // Assert
        act.Should().Throw<InlineStateException>().WithMessage("Error deserializing state: ");
    }

    private Message CreateMessageWithTextLink(string url)
    {
        return new Message
        {
            Entities = new[]
            {
                new MessageEntity
                {
                    Type = MessageEntityType.TextLink,
                    Url = url
                }
            },

            Text = "<p>Hello, world!</p>"
        };
    }
}