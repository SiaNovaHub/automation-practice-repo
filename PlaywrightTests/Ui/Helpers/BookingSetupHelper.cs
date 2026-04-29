using PlaywrightTests.Api.Clients;
using PlaywrightTests.Api.Models.Requests;

namespace PlaywrightTests.Ui.Helpers;

public static class BookingSetupHelper
{
    public static UiBookingData BuildBooking(int eventId, int quantity = 1)
    {
        return new UiBookingData
        {
            EventId = eventId,
            CustomerName = "John Doe",
            CustomerEmail = $"customer_{Guid.NewGuid():N}@test.com",
            CustomerPhone = "+91-9876543210",
            Quantity = quantity
        };
    }

    public static async Task<UiBookingData> CreateBookingAsync(ApiClientFactory apiFactory, string token, UiBookingData input)
    {
        await using var apiContext = await apiFactory.CreateContextAsync();
        var bookingApi = new BookingApi(apiContext);
        var createdBooking = await bookingApi.CreateBookingAsync(token, new CreateBookingRequest
        {
            EventId = input.EventId,
            CustomerName = input.CustomerName,
            CustomerEmail = input.CustomerEmail,
            CustomerPhone = input.CustomerPhone,
            Quantity = input.Quantity
        });

        return new UiBookingData
        {
            Id = createdBooking.Id,
            EventId = createdBooking.EventId,
            CustomerName = createdBooking.CustomerName,
            CustomerEmail = createdBooking.CustomerEmail,
            CustomerPhone = createdBooking.CustomerPhone,
            Quantity = createdBooking.Quantity,
            TotalPrice = createdBooking.TotalPrice,
            Status = createdBooking.Status,
            BookingRef = createdBooking.BookingRef
        };
    }
}
