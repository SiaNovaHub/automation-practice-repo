using PlaywrightTests.Api.Models.Requests;

namespace PlaywrightTests.Api.Tests.Infrastructure.Builders;

public static class EventBuilder
{
    public static UpsertEventRequest Valid()
    {
        return new UpsertEventRequest
        {
            Title = $"Test Event {Guid.NewGuid():N}"[..22],
            Description = "API test event",
            Category = "Conference",
            Venue = "Test Venue",
            City = "Bangalore",
            EventDate = "2026-06-15T09:00:00.000Z",
            Price = 1500,
            TotalSeats = 50,
            ImageUrl = "https://example.com/banner.jpg"
        };
    }
}