using System.Net.Http.Headers;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace Butler.Bot.AWS.Tests;

public static class TestExtensions

{
    public static HttpClient CreateAuthorizedClient<T>(this WebApplicationFactory<T> factory, string token,
        Action<IServiceCollection>? servicesConfiguration = null) where T : class
    {
        var client = CreateAnonymousClient(factory, servicesConfiguration);
        client.DefaultRequestHeaders.Add("X-Telegram-Bot-Api-Secret-Token", token); 
        return client;
    }
    
    public static HttpClient CreateAnonymousClient<T>(this WebApplicationFactory<T> factory,
        Action<IServiceCollection>? servicesConfiguration = null) where T : class
    {
        var client = factory.InitialConfigure(servicesConfiguration)
            .CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        
        
        return client;
    }
    
    public static WebApplicationFactory<T> InitialConfigure<T>(this WebApplicationFactory<T> factory,
         Action<IServiceCollection>? servicesConfiguration = null) where T : class
    {
        return factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((ctx, cfgBuilder) =>
            {
                cfgBuilder.AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.test.json"));
            });
            builder.ConfigureTestServices(services =>
            {
                servicesConfiguration?.Invoke(services);
            });
        });
    }

    public static StringContent AsBodyJson<T>(this T data)
    {
        var body = new StringContent(JsonConvert.SerializeObject(data));
        body.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        return body;
    }
    
    public static void ShouldHave200Code(this HttpResponseMessage message) =>
        ((int)message.StatusCode).Should()
        .Be(Status200OK, message.Content.ReadAsStringAsync().Result);
    
    public static void ShouldHave403Code(this HttpResponseMessage message) =>
        ((int)message.StatusCode).Should()
        .Be(Status403Forbidden, message.Content.ReadAsStringAsync().Result);
}