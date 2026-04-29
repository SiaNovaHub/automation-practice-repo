using Microsoft.Playwright;
using System.Text.Json;
using PlaywrightTests.Api.Models.Requests;
using PlaywrightTests.Api.Models.Responses;
using PlaywrightTests.Api.Tests.Infrastructure.Contexts;

namespace PlaywrightTests.Api.Tests.Infrastructure.Helpers;

public static class BookingTestHelpers
{
    public static async Task<BookingDto> CreateTestBookingAsync(BookingTestContext context, string token, CreateBookingRequest input)
    {
        return await context.Bookings.CreateBookingAsync(token, input);
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
