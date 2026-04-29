using Microsoft.Playwright;

namespace PlaywrightTests.Ui.Pages;

public class EventsPage(IPage page)
{
    private readonly IPage _page = page;

    public ILocator EventCards => _page.GetByTestId("event-card");
    public ILocator EventLinks => EventCards.Locator("a");
    public ILocator BookNowButtons => _page.GetByTestId("book-now-btn");

    public Task NavigateAsync()
    {
        return _page.GotoAsync("/events");
    }

    public ILocator GetEventCard(string title)
    {
        return EventCards.Filter(new() { HasText = title });
    }

    public ILocator GetEventLink(string title)
    {
        return GetEventCard(title).GetByRole(AriaRole.Link, new() { Name = title, Exact = true });
    }

    public ILocator GetBookNowButton(string title)
    {
        return GetEventCard(title).GetByTestId("book-now-btn");
    }

    public Task OpenEventAsync(string title)
    {
        return GetEventLink(title).ClickAsync();
    }

    public Task ClickBookNowAsync(string title)
    {
        return GetBookNowButton(title).ClickAsync();
    }
}
