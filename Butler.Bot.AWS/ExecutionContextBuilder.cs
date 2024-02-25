using Microsoft.AspNetCore.Http.Extensions;

namespace Butler.Bot.AWS;

static class ExecutionContextBuilder
{
    public static BotExecutionContext CreateContext(HttpRequest request)
    {
        return new BotExecutionContext
        {
            RootUrl = UriHelper.BuildAbsolute(request.Scheme, request.Host)
        };
    }
}
