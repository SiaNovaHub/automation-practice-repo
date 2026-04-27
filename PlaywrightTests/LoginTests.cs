using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;
using PlaywrightTests.Fixtures;

namespace PlaywrightTests;

public class LoginTests(PlaywrightFixture fixture) : IClassFixture<PlaywrightFixture>
{
    private readonly PlaywrightFixture _fixture = fixture;

    [Fact]
    public async Task EmptyLogin()
    {
        await using var context = await _fixture.CreateContextAsync();
        var page = await context.NewPageAsync();

        await page.GotoAsync("/");
        await page.GetByRole(AriaRole.Button, new() { Name = "Sign In"}).ClickAsync();

        var emailError = page.Locator("#email").Locator("xpath=..").Locator("p");
        var passwordError = page.Locator("#password").Locator("xpath=..").Locator("p");

        await Expect(emailError).ToHaveTextAsync("Enter a valid email");
        await Expect(passwordError).ToHaveTextAsync("Password must be at least 6 characters");
    }

    [Fact]
    public async Task ValidLogin()
    {
        await using var context = await _fixture.CreateContextAsync();
        var page = await context.NewPageAsync();

        var email = "testaz@test.com";
        var password = "passWord123!";

        await page.GotoAsync("/");
        await page.GetByRole(AriaRole.Textbox, new() { Name = "Email"}).FillAsync(email);
        await page.GetByRole(AriaRole.Textbox, new() { Name = "Password"}).FillAsync(password);
        await page.GetByRole(AriaRole.Button, new() { Name = "Sign In"}).ClickAsync();

        var userEmailDisplayed = page.GetByTestId("user-email-display");
        await Expect(userEmailDisplayed).ToHaveTextAsync(email);
    }

    [Fact]
    public async Task Logout()
    {
        await using var context = await _fixture.CreateContextAsync();
        var page = await context.NewPageAsync();

        var email = "testaz@test.com";
        var password = "passWord123!";

        await page.GotoAsync("/");

        await page.GetByRole(AriaRole.Textbox, new() { Name = "Email"}).FillAsync(email);
        await page.GetByRole(AriaRole.Textbox, new() { Name = "Password"}).FillAsync(password);

        await page.GetByRole(AriaRole.Button, new() { Name = "Sign In"}).ClickAsync();

        await page.GetByTestId("logout-btn").ClickAsync();

        await Expect(page.GetByRole(AriaRole.Button, new() { Name = "Sign In"})).ToBeVisibleAsync();
    }
}
