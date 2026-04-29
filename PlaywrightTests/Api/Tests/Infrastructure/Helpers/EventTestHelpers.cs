using Microsoft.Playwright;
using System.Text.Json;
using PlaywrightTests.Api.Tests.Infrastructure.Contexts;
using PlaywrightTests.Api.Models.Requests;

namespace PlaywrightTests.Api.Tests.Infrastructure.Helpers;

public static class EventTestHelpers
{
    public static async Task<int> CreateTestEventAsync(EventTestContext context, string token, UpsertEventRequest input)
    {
        var response = await context.Events.CreateRawEventAsync(token, input);
        return await ExtractIdAsync(response);
    }

    private static async Task<int> ExtractIdAsync(IAPIResponse response)
    {
        var json = await response.JsonAsync();
        return json.Value.GetProperty("data").GetProperty("id").GetInt32();
    }

    public static async Task<JsonElement> GetRootAsync(IAPIResponse response)
    {
        var json = await response.JsonAsync();
        return json.Value;
    }

    public static JsonElement GetData(JsonElement root)
    {
        return root.GetProperty("data");
    }
}