using PlaywrightTests.Api.Clients;
using PlaywrightTests.Api.Models.Requests;

namespace PlaywrightTests.Ui.Helpers;

public static class EventSetupHelper
{
    public static UiEventData BuildEvent(string? title = null, int totalSeats = 20, int price = 1500)
    {
        return new UiEventData
        {
            Title = title ?? $"UI Event {Guid.NewGuid():N}"[..20],
            Description = "UI seeded event",
            Category = "Conference",
            Venue = "Bangalore International Centre",
            City = "Bangalore",
            EventDate = "2026-06-15T09:00:00.000Z",
            Price = price,
            TotalSeats = totalSeats,
            ImageUrl = "https://example.com/banner.jpg"
        };
    }

    public static async Task<UiEventData> CreateEventAsync(ApiClientFactory apiFactory, string token, UiEventData input)
    {
        await using var apiContext = await apiFactory.CreateContextAsync();
        var eventApi = new EventApi(apiContext);
        var createdEvent = await eventApi.CreateEventAsync(token, new UpsertEventRequest
        {
            Title = input.Title,
            Description = input.Description,
            Category = input.Category,
            Venue = input.Venue,
            City = input.City,
            EventDate = input.EventDate,
            Price = input.Price,
            TotalSeats = input.TotalSeats,
            ImageUrl = input.ImageUrl
        });

        return new UiEventData
        {
            Id = createdEvent.Id,
            Title = createdEvent.Title,
            Description = createdEvent.Description,
            Category = createdEvent.Category,
            Venue = createdEvent.Venue,
            City = createdEvent.City,
            EventDate = createdEvent.EventDate.ToString("O"),
            Price = createdEvent.Price,
            TotalSeats = createdEvent.TotalSeats,
            AvailableSeats = createdEvent.AvailableSeats,
            ImageUrl = createdEvent.ImageUrl
        };
    }
}
