﻿using System.Net.Http.Headers;

namespace Butler.Bot.AWS.Tests;

public static class TestExtensions
{
    public static StringContent AsBodyJson<T>(this T data)
    {
        var body = new StringContent(JsonConvert.SerializeObject(data));
        body.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        return body;
    }

    public static async Task<T> ReadAsJsonAsync<T>(this HttpContent content)
    {
        var dataAsString = await content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<T>(dataAsString)!;
    }
}