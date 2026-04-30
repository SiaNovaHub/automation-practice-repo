using static Microsoft.Playwright.Assertions;
using PlaywrightTests.Fixtures;
using PlaywrightTests.Ui.Helpers;
using PlaywrightTests.Ui.Pages;

namespace PlaywrightTests.Ui.Tests;

[Trait("Layer", "UI")]
[Trait("Feature", "Events")]
public class EventTests(
    BrowserFixture fixture,
    ApiClientFactory apiFactory
    ) : IClassFixture<BrowserFixture>, IClassFixture<ApiClientFactory>
{
    private readonly BrowserFixture _fixture = fixture;
    private readonly ApiClientFactory _apiFactory = apiFactory;

    [Trait("Type", "Smoke")]
    [Fact]
    public async Task SeededEventAppearsInEventsList()
    {
        await using var context = await _fixture.CreateContextAsync();

        var user = await AuthHelper.RegisterAndAuthenticateAsync(context, _apiFactory);
        var eventInput = EventSetupHelper.BuildEvent();
        await EventSetupHelper.CreateEventAsync(_apiFactory, user.Token, eventInput);

        var page = await context.NewPageAsync();
        var eventsPage = new EventsPage(page);

        // Act
        await eventsPage.NavigateAsync();

        // Assert
        await Expect(eventsPage.GetEventCard(eventInput.Title)).ToBeVisibleAsync();
    }

    [Trait("Type", "Regression")]
    [Fact]
    public async Task SeededEventDetailsShowEventData()
    {
        await using var context = await _fixture.CreateContextAsync();

        var user = await AuthHelper.RegisterAndAuthenticateAsync(context, _apiFactory);
        var eventInput = EventSetupHelper.BuildEvent();
        var createdEvent = await EventSetupHelper.CreateEventAsync(_apiFactory, user.Token, eventInput);

        var page = await context.NewPageAsync();
        var eventDetailsPage = new EventDetailsPage(page);

        // Act
        await eventDetailsPage.NavigateAsync(createdEvent.Id);

        // Assert
        await Expect(eventDetailsPage.Title).ToHaveTextAsync(eventInput.Title);
        await Expect(page.GetByText(eventInput.Venue)).ToBeVisibleAsync();
        await Expect(page.GetByText(eventInput.City).First).ToBeVisibleAsync();
        Assert.Equal(eventInput.TotalSeats, await eventDetailsPage.GetAvailableSeatsAsync());
        Assert.Equal(eventInput.TotalSeats, await eventDetailsPage.GetTotalSeatsAsync());
    }

    [Trait("Type", "Smoke")]
    [Fact]
    public async Task CreateEventFromAdmin_ShowsNewEventInManageEventsTable()
    {
        await using var context = await _fixture.CreateContextAsync();

        await AuthHelper.RegisterAndAuthenticateAsync(context, _apiFactory);

        var eventInput = EventSetupHelper.BuildEvent();
        var page = await context.NewPageAsync();
        var adminEventsPage = new AdminEventsPage(page);

        // Act
        await adminEventsPage.NavigateAsync();
        await adminEventsPage.CreateEventAsync(eventInput);

        // Assert
        await Expect(adminEventsPage.GetEventRowByTitle(eventInput.Title)).ToBeVisibleAsync();
        await Expect(adminEventsPage.GetEventRowByTitle(eventInput.Title)).ToContainTextAsync(eventInput.City);
        await Expect(adminEventsPage.GetEventRowByTitle(eventInput.Title)).ToContainTextAsync("20/20");
    }
}
