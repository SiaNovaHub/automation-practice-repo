using System.Text.Json;
using PlaywrightTests.Api.Models.Queries;
using PlaywrightTests.Api.Models.Requests;
using PlaywrightTests.Api.Tests.Infrastructure.Contexts;
using PlaywrightTests.Api.Tests.Infrastructure.Helpers;
using PlaywrightTests.Api.Tests.Infrastructure.Auth;
using PlaywrightTests.Api.Tests.Infrastructure.Builders;

namespace PlaywrightTests.Api.Tests.Features.Bookings;

[Trait("Layer", "API")]
[Trait("Feature", "Bookings")]
public class BookingsTests(ApiClientFactory factory) : IClassFixture<ApiClientFactory>
{
    private readonly ApiClientFactory _factory = factory;

    private async Task<BookingTestContext> CreateContextAsync()
    {
        var request = await _factory.CreateContextAsync();
        return new BookingTestContext(request);
    }

    private static bool ContainsValidationError(JsonElement details, string field, string message)
    {
        foreach (var detail in details.EnumerateArray())
        {
            if (detail.GetProperty("field").GetString() == field &&
                detail.GetProperty("message").GetString() == message)
            {
                return true;
            }
        }

        return false;
    }

    [Trait("Type", "Smoke")]
    [Fact]
    public async Task ListBookings_Returns200()
    {
        await using var context = await CreateContextAsync();
        var token = await AuthHelper.CreateUserTokenAsync(context.Auth);

        // Act
        var response = await context.Bookings.ListRawAsync(token);

        // Assert
        Assert.Equal(200, response.Status);
    }

    [Trait("Type", "Regression")]
    [Fact]
    public async Task ListBookings_ReturnsPaginatedCollection()
    {
        await using var context = await CreateContextAsync();
        var token = await AuthHelper.CreateUserTokenAsync(context.Auth);
        // Act
        var response = await context.Bookings.ListBookingsAsync(token);

        // Assert
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.NotNull(response.Pagination);
        Assert.True(response.Pagination.Page >= 1);
        Assert.True(response.Pagination.Limit >= 1);
    }

    [Trait("Type", "Regression")]
    [Fact]
    public async Task ListBookings_FilteredByEventId_ReturnsMatchingBookings()
    {
        await using var context = await CreateContextAsync();
        var token = await AuthHelper.CreateUserTokenAsync(context.Auth);
        var eventId = 0;
        var bookingId = 0;

        try
        {
            eventId = await EventTestHelpers.CreateTestEventAsync(context.Events, token, EventBuilder.Valid());
            var booking = await BookingTestHelpers.CreateTestBookingAsync(context, token, BookingBuilder.Valid(eventId));
            bookingId = booking.Id;

            // Act
            var response = await context.Bookings.ListBookingsAsync(token, new BookingQuery { EventId = eventId });

            // Assert
            Assert.True(response.Success);
            Assert.Contains(response.Data, item => item.Id == bookingId);
            Assert.All(response.Data, item => Assert.Equal(eventId, item.EventId));
        }
        finally
        {
            if (bookingId > 0)
            {
                await context.Bookings.DeleteRawAsync(token, bookingId);
            }

            if (eventId > 0)
            {
                await context.Events.DeleteRawAsync(token, eventId);
            }
        }
    }

    [Trait("Type", "Smoke")]
    [Fact]
    public async Task CreateBooking_WithValidData_Returns201()
    {
        await using var context = await CreateContextAsync();
        var token = await AuthHelper.CreateUserTokenAsync(context.Auth);
        var eventId = 0;
        var bookingId = 0;

        try
        {
            eventId = await EventTestHelpers.CreateTestEventAsync(context.Events, token, EventBuilder.Valid());

            // Act
            var response = await context.Bookings.CreateRawAsync(token, BookingBuilder.Valid(eventId));

            // Assert
            Assert.Equal(201, response.Status);

            var root = await BookingTestHelpers.GetRootAsync(response);
            bookingId = BookingTestHelpers.GetData(root).GetProperty("id").GetInt32();
        }
        finally
        {
            if (bookingId > 0)
            {
                await context.Bookings.DeleteRawAsync(token, bookingId);
            }

            if (eventId > 0)
            {
                await context.Events.DeleteRawAsync(token, eventId);
            }
        }
    }

    [Trait("Type", "Regression")]
    [Fact]
    public async Task CreateBooking_WithValidData_ReturnsExpectedContract()
    {
        await using var context = await CreateContextAsync();
        var token = await AuthHelper.CreateUserTokenAsync(context.Auth);
        var eventId = 0;
        var bookingId = 0;

        try
        {
            eventId = await EventTestHelpers.CreateTestEventAsync(context.Events, token, EventBuilder.Valid());

            // Act
            var response = await context.Bookings.CreateBookingResponseAsync(token, BookingBuilder.Valid(eventId));
            bookingId = response.Data.Id;

            // Assert
            Assert.True(response.Success);
            Assert.Equal("Booking confirmed!", response.Message);
            Assert.NotNull(response.Data);
            Assert.NotNull(response.Data.Event);
            Assert.False(string.IsNullOrEmpty(response.Data.BookingRef));
        }
        finally
        {
            if (bookingId > 0)
            {
                await context.Bookings.DeleteRawAsync(token, bookingId);
            }

            if (eventId > 0)
            {
                await context.Events.DeleteRawAsync(token, eventId);
            }
        }
    }

    [Trait("Type", "Regression")]
    [Fact]
    public async Task CreateBooking_WithValidData_ReturnsCalculatedDataAndUpdatesSeats()
    {
        await using var context = await CreateContextAsync();
        var token = await AuthHelper.CreateUserTokenAsync(context.Auth);
        var eventInput = EventBuilder.Valid();
        var eventId = 0;
        var bookingId = 0;
        var quantity = 2;

        try
        {
            eventId = await EventTestHelpers.CreateTestEventAsync(context.Events, token, eventInput);
            var bookingInput = BookingBuilder.Valid(eventId, quantity);

            // Act
            var response = await context.Bookings.CreateBookingResponseAsync(token, bookingInput);
            bookingId = response.Data.Id;
            var updatedEvent = await context.Events.GetEventAsync(token, eventId);

            // Assert
            Assert.Equal(eventId, response.Data.EventId);
            Assert.Equal(quantity, response.Data.Quantity);
            Assert.Equal(eventInput.Price * quantity, response.Data.TotalPrice);
            Assert.Equal("confirmed", response.Data.Status);
            Assert.Equal(eventInput.TotalSeats - quantity, updatedEvent.AvailableSeats);
        }
        finally
        {
            if (bookingId > 0)
            {
                await context.Bookings.DeleteRawAsync(token, bookingId);
            }

            if (eventId > 0)
            {
                await context.Events.DeleteRawAsync(token, eventId);
            }
        }
    }

    [Trait("Type", "Regression")]
    [Fact]
    public async Task CreateBooking_WithInvalidData_Returns400()
    {
        await using var context = await CreateContextAsync();
        var token = await AuthHelper.CreateUserTokenAsync(context.Auth);
        var eventId = 0;

        try
        {
            eventId = await EventTestHelpers.CreateTestEventAsync(context.Events, token, EventBuilder.Valid());
            var input = BookingBuilder.Valid(eventId, 0);
            input.CustomerName = "P";
            input.CustomerEmail = "bad";
            input.CustomerPhone = "123";

            // Act
            var response = await context.Bookings.CreateRawAsync(token, input);

            // Assert
            Assert.Equal(400, response.Status);

            var root = await BookingTestHelpers.GetRootAsync(response);
            var details = root.GetProperty("details");

            Assert.False(root.GetProperty("success").GetBoolean());
            Assert.Equal("Validation failed", root.GetProperty("error").GetString());
            Assert.True(ContainsValidationError(details, "customerName", "Customer name must be at least 2 characters"));
            Assert.True(ContainsValidationError(details, "customerEmail", "Customer email must be a valid email address"));
            Assert.True(ContainsValidationError(details, "customerPhone", "Customer phone must be at least 10 digits"));
            Assert.True(ContainsValidationError(details, "quantity", "Quantity must be an integer between 1 and 10"));
        }
        finally
        {
            if (eventId > 0)
            {
                await context.Events.DeleteRawAsync(token, eventId);
            }
        }
    }

    [Trait("Type", "Smoke")]
    [Fact]
    public async Task GetBookingById_WithExistingId_ReturnsExpectedBooking()
    {
        await using var context = await CreateContextAsync();
        var token = await AuthHelper.CreateUserTokenAsync(context.Auth);
        var eventId = 0;
        var bookingId = 0;

        try
        {
            eventId = await EventTestHelpers.CreateTestEventAsync(context.Events, token, EventBuilder.Valid());
            var createdBooking = await BookingTestHelpers.CreateTestBookingAsync(context, token, BookingBuilder.Valid(eventId));
            bookingId = createdBooking.Id;

            // Act
            var response = await context.Bookings.GetBookingResponseAsync(token, bookingId);

            // Assert
            Assert.True(response.Success);
            Assert.Equal(bookingId, response.Data.Id);
            Assert.Equal(eventId, response.Data.EventId);
            Assert.Equal("John Doe", response.Data.CustomerName);
            Assert.Equal("confirmed", response.Data.Status);
        }
        finally
        {
            if (bookingId > 0)
            {
                await context.Bookings.DeleteRawAsync(token, bookingId);
            }

            if (eventId > 0)
            {
                await context.Events.DeleteRawAsync(token, eventId);
            }
        }
    }

    [Trait("Type", "Regression")]
    [Fact]
    public async Task GetBookingById_WithUnknownId_Returns404()
    {
        await using var context = await CreateContextAsync();
        var token = await AuthHelper.CreateUserTokenAsync(context.Auth);
        var bookingId = 999999;

        // Act
        var response = await context.Bookings.GetRawAsync(token, bookingId);

        // Assert
        Assert.Equal(404, response.Status);

        var root = await BookingTestHelpers.GetRootAsync(response);
        Assert.False(root.GetProperty("success").GetBoolean());
        Assert.Equal($"Booking with id {bookingId} not found", root.GetProperty("error").GetString());
    }

    [Trait("Type", "Regression")]
    [Fact]
    public async Task GetBookingByReference_WithExistingReference_ReturnsExpectedBooking()
    {
        await using var context = await CreateContextAsync();
        var token = await AuthHelper.CreateUserTokenAsync(context.Auth);
        var eventId = 0;
        var bookingId = 0;

        try
        {
            eventId = await EventTestHelpers.CreateTestEventAsync(context.Events, token, EventBuilder.Valid());
            var createdBooking = await BookingTestHelpers.CreateTestBookingAsync(context, token, BookingBuilder.Valid(eventId));
            bookingId = createdBooking.Id;

            // Act
            var response = await context.Bookings.GetBookingByReferenceResponseAsync(token, createdBooking.BookingRef);

            // Assert
            Assert.True(response.Success);
            Assert.Equal(createdBooking.BookingRef, response.Data.BookingRef);
            Assert.Equal(bookingId, response.Data.Id);
            Assert.Equal(eventId, response.Data.EventId);
            Assert.Equal("John Doe", response.Data.CustomerName);
        }
        finally
        {
            if (bookingId > 0)
            {
                await context.Bookings.DeleteRawAsync(token, bookingId);
            }

            if (eventId > 0)
            {
                await context.Events.DeleteRawAsync(token, eventId);
            }
        }
    }

    [Trait("Type", "Regression")]
    [Fact]
    public async Task GetBookingByReference_WithUnknownReference_Returns404()
    {
        await using var context = await CreateContextAsync();
        var token = await AuthHelper.CreateUserTokenAsync(context.Auth);
        var reference = "EVT-XYZ123";

        // Act
        var response = await context.Bookings.GetByReferenceRawAsync(token, reference);

        // Assert
        Assert.Equal(404, response.Status);

        var root = await BookingTestHelpers.GetRootAsync(response);
        Assert.False(root.GetProperty("success").GetBoolean());
        Assert.Equal($"Booking with reference \"{reference}\" not found", root.GetProperty("error").GetString());
    }

    [Trait("Type", "Smoke")]
    [Fact]
    public async Task DeleteBooking_WithExistingId_Returns200AndRestoresSeats()
    {
        await using var context = await CreateContextAsync();
        var token = await AuthHelper.CreateUserTokenAsync(context.Auth);
        var eventInput = EventBuilder.Valid();
        var eventId = 0;
        var bookingId = 0;
        var quantity = 2;

        try
        {
            eventId = await EventTestHelpers.CreateTestEventAsync(context.Events, token, eventInput);
            var booking = await BookingTestHelpers.CreateTestBookingAsync(context, token, BookingBuilder.Valid(eventId, quantity));
            bookingId = booking.Id;

            var bookedEvent = await context.Events.GetEventAsync(token, eventId);

            // Act
            var response = await context.Bookings.DeleteRawAsync(token, bookingId);
            var root = await BookingTestHelpers.GetRootAsync(response);
            var restoredEvent = await context.Events.GetEventAsync(token, eventId);
            bookingId = 0;

            // Assert
            Assert.Equal(200, response.Status);
            Assert.True(root.GetProperty("success").GetBoolean());
            Assert.Equal("Booking cancelled", root.GetProperty("message").GetString());
            Assert.Equal(eventInput.TotalSeats - quantity, bookedEvent.AvailableSeats);
            Assert.Equal(eventInput.TotalSeats, restoredEvent.AvailableSeats);
        }
        finally
        {
            if (bookingId > 0)
            {
                await context.Bookings.DeleteRawAsync(token, bookingId);
            }

            if (eventId > 0)
            {
                await context.Events.DeleteRawAsync(token, eventId);
            }
        }
    }

    [Trait("Type", "Regression")]
    [Fact]
    public async Task DeleteBooking_WithUnknownId_Returns404()
    {
        await using var context = await CreateContextAsync();
        var token = await AuthHelper.CreateUserTokenAsync(context.Auth);
        var bookingId = 999999;

        // Act
        var response = await context.Bookings.DeleteRawAsync(token, bookingId);

        // Assert
        Assert.Equal(404, response.Status);

        var root = await BookingTestHelpers.GetRootAsync(response);
        Assert.False(root.GetProperty("success").GetBoolean());
        Assert.Equal($"Booking with id {bookingId} not found", root.GetProperty("error").GetString());
    }
}
