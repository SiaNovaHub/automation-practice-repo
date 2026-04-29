using Microsoft.Playwright;
using PlaywrightTests.Ui.Helpers;

namespace PlaywrightTests.Ui.Pages;

public class AdminEventsPage(IPage page)
{
    private readonly IPage _page = page;

    public ILocator TitleInput => _page.GetByTestId("event-title-input");
    public ILocator DescriptionInput => _page.GetByRole(AriaRole.Textbox, new() { Name = "Describe the event…" });
    public ILocator CategorySelect => _page.GetByRole(AriaRole.Combobox, new() { Name = "Category*" });
    public ILocator CityInput => _page.GetByRole(AriaRole.Textbox, new() { Name = "City*" });
    public ILocator VenueInput => _page.GetByRole(AriaRole.Textbox, new() { Name = "Venue*" });
    public ILocator EventDateInput => _page.GetByRole(AriaRole.Textbox, new() { Name = "Event Date & Time*" });
    public ILocator PriceInput => _page.GetByRole(AriaRole.Spinbutton, new() { Name = "Price ($)*" });
    public ILocator TotalSeatsInput => _page.GetByRole(AriaRole.Spinbutton, new() { Name = "Total Seats*" });
    public ILocator ImageUrlInput => _page.GetByRole(AriaRole.Textbox, new() { Name = "Image URL (optional)" });
    public ILocator AddEventButton => _page.GetByTestId("add-event-btn");
    public ILocator EventRows => _page.GetByRole(AriaRole.Row);

    public ILocator GetEventRowByTitle(string title)
    {
        return EventRows.Filter(new() { HasText = title });
    }

    public Task NavigateAsync()
    {
        return _page.GotoAsync("/admin/events");
    }

    public async Task CreateEventAsync(UiEventData input)
    {
        await TitleInput.FillAsync(input.Title);
        await DescriptionInput.FillAsync(input.Description);
        await CategorySelect.SelectOptionAsync(input.Category);
        await CityInput.FillAsync(input.City);
        await VenueInput.FillAsync(input.Venue);
        await EventDateInput.FillAsync(FormatEventDate(input.EventDate));
        await PriceInput.FillAsync(input.Price.ToString());
        await TotalSeatsInput.FillAsync(input.TotalSeats.ToString());

        if (!string.IsNullOrWhiteSpace(input.ImageUrl))
        {
            await ImageUrlInput.FillAsync(input.ImageUrl);
        }

        await AddEventButton.ClickAsync();
    }

    private static string FormatEventDate(string eventDate)
    {
        return eventDate.EndsWith("Z") && eventDate.Length >= 16
            ? eventDate[..16]
            : eventDate;
    }
}
