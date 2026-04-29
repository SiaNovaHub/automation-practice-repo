using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;
using PlaywrightTests.Fixtures;
using PlaywrightTests.Ui.Helpers;

namespace PlaywrightTests.Ui.Tests;

public class LoginTests(
    BrowserFixture fixture,
    ApiClientFactory apiFactory
    ) : IClassFixture<BrowserFixture>, IClassFixture<ApiClientFactory>
{
    private readonly BrowserFixture _fixture = fixture;
    private readonly ApiClientFactory _apiFactory = apiFactory;

    [Fact]
    public async Task EmptyLogin()
    {
        await using var context = await _fixture.CreateContextAsync();
        var page = await context.NewPageAsync();

        await page.GotoAsync("/");
        await page.GetByRole(AriaRole.Button, new() { Name = "Sign In" }).ClickAsync();

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

        //Act
        await page.GotoAsync("/");
        await page.GetByRole(AriaRole.Textbox, new() { Name = "Email" }).FillAsync(email);
        await page.GetByRole(AriaRole.Textbox, new() { Name = "Password" }).FillAsync(password);
        await page.GetByRole(AriaRole.Button, new() { Name = "Sign In" }).ClickAsync();

        //Assert
        var userEmailDisplayed = page.GetByTestId("user-email-display");
        await page.WaitForURLAsync("**/");
        await Expect(userEmailDisplayed).ToHaveTextAsync(email);
    }

    [Fact]
    public async Task Logout()
    {
        await using var context = await _fixture.CreateContextAsync();

        var email = "testaz@test.com";
        var password = "passWord123!";

        //Arrange
        await AuthHelper.LoginViaApiAsync(context, _apiFactory, email, password);

        var page = await context.NewPageAsync();

        //Act
        await page.GotoAsync("/");

        await page.GetByRole(AriaRole.Textbox, new() { Name = "Email" }).FillAsync(email);
        await page.GetByRole(AriaRole.Textbox, new() { Name = "Password" }).FillAsync(password);

        await page.GetByRole(AriaRole.Button, new() { Name = "Sign In" }).ClickAsync();

        await page.GetByTestId("logout-btn").ClickAsync();

        //Assert
        await page.WaitForURLAsync("**/login");
        await Expect(page.GetByRole(AriaRole.Button, new() { Name = "Sign In" })).ToBeVisibleAsync();
    }
}
