using Microsoft.Playwright;

namespace PlaywrightTests.Ui.Pages;

public class MyBookingsPage(IPage page)
{
    private readonly IPage _page = page;

    public ILocator CancelBookingButtons => _page.GetByTestId("cancel-booking-btn");
    public ILocator ConfirmCancellationButton => _page.GetByRole(AriaRole.Button, new() { Name = "Yes, cancel it" });

    public ILocator GetBookingCard(string eventTitle)
    {
        return _page
            .GetByRole(AriaRole.Heading, new() { Name = eventTitle, Exact = true })
            .Locator("xpath=ancestor::div[.//button[@data-testid='cancel-booking-btn']][1]");
    }

    public ILocator GetCancelBookingButton(string eventTitle)
    {
        return GetBookingCard(eventTitle).GetByTestId("cancel-booking-btn");
    }

    public Task NavigateAsync()
    {
        return _page.GotoAsync("/bookings");
    }

    public async Task CancelBookingAsync(string eventTitle)
    {
        await GetCancelBookingButton(eventTitle).ClickAsync();
        await ConfirmCancellationButton.ClickAsync();
    }
}
