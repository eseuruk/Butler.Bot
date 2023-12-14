using System.Net;

namespace Butler.Bot.AWS.Tests;

public class TestMessageHandler : HttpMessageHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var response = new HttpResponseMessage(HttpStatusCode.OK);
        return Task.FromResult(response);
    }
}
