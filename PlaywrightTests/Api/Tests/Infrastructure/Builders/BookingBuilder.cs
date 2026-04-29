using PlaywrightTests.Api.Models.Requests;

namespace PlaywrightTests.Api.Tests.Infrastructure.Builders;

public static class BookingBuilder
{
    public static CreateBookingRequest Valid(int eventId, int quantity = 2)
    {
        return new CreateBookingRequest
        {
            EventId = eventId,
            CustomerName = "John Doe",
            CustomerEmail = "john.doe@email.com",
            CustomerPhone = "+91-9876543210",
            Quantity = quantity
        };
    }
}