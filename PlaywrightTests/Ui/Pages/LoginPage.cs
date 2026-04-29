using Microsoft.Playwright;

namespace PlaywrightTests.Ui.Pages;

public class LoginPage(IPage page)
{
    private readonly IPage _page = page;

    public ILocator EmailInput => _page.GetByRole(AriaRole.Textbox, new() { Name = "Email" });
    public ILocator PasswordInput => _page.GetByRole(AriaRole.Textbox, new() { Name = "Password" });
    public ILocator SignInButton => _page.GetByRole(AriaRole.Button, new() { Name = "Sign In" });
    public ILocator EmailError => _page.Locator("#email").Locator("xpath=..").Locator("p");
    public ILocator PasswordError => _page.Locator("#password").Locator("xpath=..").Locator("p");

    public Task NavigateAsync()
    {
        return _page.GotoAsync("/login");
    }

    public async Task LoginAsync(string email, string password)
    {
        await EmailInput.FillAsync(email);
        await PasswordInput.FillAsync(password);
        await SignInButton.ClickAsync();
    }

    public Task SubmitAsync()
    {
        return SignInButton.ClickAsync();
    }
}
