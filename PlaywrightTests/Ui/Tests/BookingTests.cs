using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;
using PlaywrightTests.Fixtures;
using PlaywrightTests.Ui.Helpers;
using PlaywrightTests.Ui.Pages;

namespace PlaywrightTests.Ui.Tests;

public class BookingTests(
    BrowserFixture fixture,
    ApiClientFactory apiFactory
    ) : IClassFixture<BrowserFixture>, IClassFixture<ApiClientFactory>
{
    private readonly BrowserFixture _fixture = fixture;
    private readonly ApiClientFactory _apiFactory = apiFactory;

    [Fact]
    public async Task CreateBookingFromEventDetails_ShowsBookingConfirmation()
    {
        await using var context = await _fixture.CreateContextAsync();

        var user = await AuthHelper.RegisterAndAuthenticateAsync(context, _apiFactory);
        var eventInput = EventSetupHelper.BuildEvent();
        var createdEvent = await EventSetupHelper.CreateEventAsync(_apiFactory, user.Token, eventInput);

        var page = await context.NewPageAsync();
        var eventDetailsPage = new EventDetailsPage(page);

        // Act
        await eventDetailsPage.NavigateAsync(createdEvent.Id);
        await eventDetailsPage.BookTicketsAsync("Priya Sharma", "priya.sharma@email.com", "+91-9876543210");

        // Assert
        await Expect(eventDetailsPage.BookingConfirmedHeading).ToBeVisibleAsync();
        Assert.False(string.IsNullOrEmpty(await eventDetailsPage.GetBookingReferenceAsync()));
    }

    [Fact]
    public async Task SeededBookingAppearsInMyBookings()
    {
        await using var context = await _fixture.CreateContextAsync();

        var user = await AuthHelper.RegisterAndAuthenticateAsync(context, _apiFactory);
        var eventInput = EventSetupHelper.BuildEvent();
        var createdEvent = await EventSetupHelper.CreateEventAsync(_apiFactory, user.Token, eventInput);
        var createdBooking = await BookingSetupHelper.CreateBookingAsync(_apiFactory, user.Token, BookingSetupHelper.BuildBooking(createdEvent.Id));

        var page = await context.NewPageAsync();
        var myBookingsPage = new MyBookingsPage(page);

        // Act
        await myBookingsPage.NavigateAsync();

        // Assert
        await Expect(myBookingsPage.GetBookingCard(eventInput.Title)).ToBeVisibleAsync();
        await Expect(myBookingsPage.GetBookingCard(eventInput.Title)).ToContainTextAsync(createdBooking.BookingRef);
    }

    [Fact]
    public async Task CreatingBookingDecreasesAvailableSeatsAfterReload()
    {
        await using var context = await _fixture.CreateContextAsync();

        var user = await AuthHelper.RegisterAndAuthenticateAsync(context, _apiFactory);
        var eventInput = EventSetupHelper.BuildEvent(totalSeats: 20, price: 1200);
        var createdEvent = await EventSetupHelper.CreateEventAsync(_apiFactory, user.Token, eventInput);

        var page = await context.NewPageAsync();
        var eventDetailsPage = new EventDetailsPage(page);

        // Act
        await eventDetailsPage.NavigateAsync(createdEvent.Id);
        var initialAvailableSeats = await eventDetailsPage.GetAvailableSeatsAsync();
        await eventDetailsPage.BookTicketsAsync("Priya Sharma", "priya.sharma@email.com", "+91-9876543210", quantity: 2);
        await eventDetailsPage.ReloadAsync();
        var updatedAvailableSeats = await eventDetailsPage.GetAvailableSeatsAsync();

        // Assert
        Assert.Equal(eventInput.TotalSeats, initialAvailableSeats);
        Assert.Equal(eventInput.TotalSeats - 2, updatedAvailableSeats);
    }

    [Fact]
    public async Task CancellingBookingRestoresAvailableSeats()
    {
        await using var context = await _fixture.CreateContextAsync();

        var user = await AuthHelper.RegisterAndAuthenticateAsync(context, _apiFactory);
        var eventInput = EventSetupHelper.BuildEvent(totalSeats: 20, price: 1200);
        var createdEvent = await EventSetupHelper.CreateEventAsync(_apiFactory, user.Token, eventInput);
        var createdBooking = await BookingSetupHelper.CreateBookingAsync(_apiFactory, user.Token, BookingSetupHelper.BuildBooking(createdEvent.Id, quantity: 2));

        var page = await context.NewPageAsync();
        var myBookingsPage = new MyBookingsPage(page);
        var eventDetailsPage = new EventDetailsPage(page);

        // Act
        await myBookingsPage.NavigateAsync();
        await myBookingsPage.CancelBookingAsync(eventInput.Title);

        // Assert bookings page
        await Expect(myBookingsPage.GetBookingCard(eventInput.Title)).ToHaveCountAsync(0);

        // Assert event details page
        await eventDetailsPage.NavigateAsync(createdEvent.Id);
        Assert.Equal(eventInput.TotalSeats, await eventDetailsPage.GetAvailableSeatsAsync());
        Assert.False(string.IsNullOrEmpty(createdBooking.BookingRef));
    }
}
