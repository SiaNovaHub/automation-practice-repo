using Microsoft.Playwright;

namespace PlaywrightTests.Ui.Pages;

public class NavigationBar(IPage page)
{
    private readonly IPage _page = page;

    public ILocator UserEmail => _page.GetByTestId("user-email-display");
    public ILocator LogoutButton => _page.GetByTestId("logout-btn");

    public Task LogoutAsync()
    {
        return LogoutButton.ClickAsync();
    }
}
