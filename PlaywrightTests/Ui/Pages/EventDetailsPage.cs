using System.Text.RegularExpressions;
using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace PlaywrightTests.Ui.Pages;

public class EventDetailsPage(IPage page)
{
    private readonly IPage _page = page;

    public ILocator MainContent => _page.Locator("main");
    public ILocator Title => _page.GetByRole(AriaRole.Heading, new() { Level = 1 });
    public ILocator FullNameInput => _page.GetByRole(AriaRole.Textbox, new() { Name = "Full Name*" });
    public ILocator EmailInput => _page.GetByTestId("customer-email");
    public ILocator PhoneInput => _page.GetByRole(AriaRole.Textbox, new() { Name = "Phone Number*" });
    public ILocator IncreaseTicketsButton => _page.GetByRole(AriaRole.Button, new() { Name = "+" });
    public ILocator ConfirmBookingButton => _page.GetByRole(AriaRole.Button, new() { Name = "Confirm Booking" });
    public ILocator BookingConfirmedHeading => _page.GetByRole(AriaRole.Heading, new() { Name = "Booking Confirmed! 🎉" });

    public async Task NavigateAsync(int eventId)
    {
        await _page.GotoAsync($"/events/{eventId}");
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await Expect(Title).ToBeVisibleAsync();
    }

    public async Task BookTicketsAsync(string fullName, string email, string phoneNumber, int quantity = 1)
    {
        for (var index = 1; index < quantity; index++)
        {
            await IncreaseTicketsButton.ClickAsync();
        }

        await FullNameInput.FillAsync(fullName);
        await EmailInput.FillAsync(email);
        await PhoneInput.FillAsync(phoneNumber);
        await ConfirmBookingButton.ClickAsync();
    }

    public async Task<int> GetAvailableSeatsAsync()
    {
        await Expect(MainContent).ToBeVisibleAsync();
        var text = await MainContent.InnerTextAsync();
        var match = Regex.Match(text, @"available\s+(\d+)\s*/\s*(\d+)\s+seats", RegexOptions.IgnoreCase);

        if (!match.Success)
        {
            throw new Exception($"Could not read available seats from page text: {text}");
        }

        return int.Parse(match.Groups[1].Value);
    }

    public async Task<int> GetTotalSeatsAsync()
    {
        await Expect(MainContent).ToBeVisibleAsync();
        var text = await MainContent.InnerTextAsync();
        var match = Regex.Match(text, @"available\s+(\d+)\s*/\s*(\d+)\s+seats", RegexOptions.IgnoreCase);

        if (!match.Success)
        {
            throw new Exception($"Could not read total seats from page text: {text}");
        }

        return int.Parse(match.Groups[2].Value);
    }

    public async Task<string> GetBookingReferenceAsync()
    {
        await Expect(MainContent).ToBeVisibleAsync();
        var text = await MainContent.InnerTextAsync();
        var match = Regex.Match(text, @"Booking Ref\s+([A-Z]-[A-Z0-9]+)", RegexOptions.IgnoreCase);

        if (!match.Success)
        {
            throw new Exception($"Could not read booking reference from page text: {text}");
        }

        return match.Groups[1].Value;
    }

    public async Task ReloadAsync()
    {
        await _page.ReloadAsync();
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await Expect(Title).ToBeVisibleAsync();
    }
}
