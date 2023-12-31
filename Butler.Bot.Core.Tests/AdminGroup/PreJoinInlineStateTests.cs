﻿namespace Butler.Bot.Core.Tests.AdminGroup;

public class PreJoinInlineStateTests
{
    private readonly InlineStateManager inlineStateManager;

    public PreJoinInlineStateTests()
    {
        inlineStateManager = new InlineStateManager(NullLogger<InlineStateManager>.Instance);
    }

    [Fact]
    public void InjectStateIntoMessageHtml_ShouldReturnMessageWithStateLink()
    {
        // Arrange
        var state = new PreJoinInlineState { UserId = 653298134 };
        var messageHtml = "<p>Hello, world!</p>";

        // Act
        var resultHtml = inlineStateManager.InjectStateIntoMessageHtml(messageHtml, state);

        // Assert

        resultHtml.Should().Be("<a href='tg://btn/eyJ2IjoxLCJ1aWQiOjY1MzI5ODEzNH0='>​</a><p>Hello, world!</p>");
    }

    [Fact]
    public void GetStateFromMessage_ShouldReturnDeserializedState()
    {
        // Arrange
        var message = CreateMessageWithTextLink("tg://btn/eyJ2IjoxLCJ1aWQiOjY1MzI5ODEzNH0=");

        // Act
        var resultState = inlineStateManager.GetStateFromMessage<PreJoinInlineState>(message);

        // Assert
        var state = new PreJoinInlineState { UserId = 653298134 };
        resultState.Should().BeEquivalentTo(state);
    }

    [Fact]
    public void GetStateFromMessage_ShouldThrowInlineStateExceptionIfStateLinkNotFound()
    {
        // Arrange
        var message = CreateMessageWithTextLink("https://example.com");

        // Act
        var act = () => inlineStateManager.GetStateFromMessage<PreJoinInlineState>(message);

        // Assert
        act.Should().Throw<InlineStateException>().WithMessage("Inline state link is not found in the message*");
    }

    [Fact]
    public void GetStateFromMessage_ShouldThrowInlineStateExceptionOnInvalidBase64Data()
    {
        // Arrange
        var message = CreateMessageWithTextLink($"tg://btn/InvalidBase64Data");

        // Act
        var act = () => inlineStateManager.GetStateFromMessage<PreJoinInlineState>(message);

        // Assert
        act.Should().Throw<InlineStateException>().WithMessage("Error parsing base64*");
    }

    [Fact]
    public void GetStateFromMessage_ShouldThrowInlineStateExceptionOnInvalidJson()
    {
        // Arrange
        var message = CreateMessageWithTextLink($"tg://btn/SW52YWxpZEpzb24=");

        // Act
        var act = () => inlineStateManager.GetStateFromMessage<PreJoinInlineState>(message);

        // Assert
        act.Should().Throw<InlineStateException>().WithMessage("Error deserializing state: InvalidJson");
    }

    [Fact]
    public void GetStateFromMessage_ShouldThrowInlineStateExceptionOnEmptyJson()
    {
        // Arrange
        var message = CreateMessageWithTextLink($"tg://btn/");

        // Act
        var act = () => inlineStateManager.GetStateFromMessage<PreJoinInlineState>(message);

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