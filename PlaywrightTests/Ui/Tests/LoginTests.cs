using static Microsoft.Playwright.Assertions;
using PlaywrightTests.Fixtures;
using PlaywrightTests.Ui.Helpers;
using PlaywrightTests.Ui.Pages;

namespace PlaywrightTests.Ui.Tests;

[Trait("Layer", "UI")]
[Trait("Feature", "Auth")]
[Collection("UI collection")]
public class LoginTests : UiBaseTest
{
    public LoginTests(BrowserFixture fixture, ApiClientFactory apiClientFactory)
        : base(fixture, apiClientFactory)
    {
    }

    [Trait("Type", "Regression")]
    [Fact]
    public async Task EmptyLogin()
    {
        var context = Context;
        var page = await context.NewPageAsync();
        var loginPage = new LoginPage(page);

        // Act
        await loginPage.NavigateAsync();
        await loginPage.SubmitAsync();

        // Assert
        await Expect(loginPage.EmailError).ToHaveTextAsync("Enter a valid email");
        await Expect(loginPage.PasswordError).ToHaveTextAsync("Password must be at least 6 characters");
    }

    [Trait("Type", "Smoke")]
    [Fact]
    public async Task ValidLogin()
    {
        var context = Context;
        var user = await AuthHelper.RegisterUserAsync(ApiClientFactory);
        var page = await context.NewPageAsync();
        var loginPage = new LoginPage(page);
        var navigationBar = new NavigationBar(page);

        // Act
        await loginPage.NavigateAsync();
        await loginPage.LoginAsync(user.Email, user.Password);

        // Assert
        await page.WaitForURLAsync("**/");
        await Expect(navigationBar.UserEmail).ToHaveTextAsync(user.Email);
    }

    [Trait("Type", "Smoke")]
    [Fact]
    public async Task Logout()
    {
        var context = Context;

        // Arrange
        await AuthHelper.RegisterAndAuthenticateAsync(context, ApiClientFactory);

        var page = await context.NewPageAsync();
        var navigationBar = new NavigationBar(page);
        var loginPage = new LoginPage(page);

        await page.GotoAsync("/");

        // Act
        await navigationBar.LogoutAsync();

        // Assert
        await page.WaitForURLAsync("**/login");
        await Expect(loginPage.SignInButton).ToBeVisibleAsync();
    }
}
