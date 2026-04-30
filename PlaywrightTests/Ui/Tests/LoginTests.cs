using static Microsoft.Playwright.Assertions;
using PlaywrightTests.Fixtures;
using PlaywrightTests.Ui.Helpers;
using PlaywrightTests.Ui.Pages;

namespace PlaywrightTests.Ui.Tests;

[Trait("Layer", "UI")]
[Trait("Feature", "Auth")]
public class LoginTests(
    BrowserFixture fixture,
    ApiClientFactory apiFactory
    ) : IClassFixture<BrowserFixture>, IClassFixture<ApiClientFactory>
{
    private readonly BrowserFixture _fixture = fixture;
    private readonly ApiClientFactory _apiFactory = apiFactory;

    [Trait("Type", "Regression")]
    [Fact]
    public async Task EmptyLogin()
    {
        await using var context = await _fixture.CreateContextAsync();

        try
        {
            var page = await context.NewPageAsync();
            var loginPage = new LoginPage(page);

            // Act
            await loginPage.NavigateAsync();
            await loginPage.SubmitAsync();

            // Assert
            await Expect(loginPage.EmailError).ToHaveTextAsync("Enter a valid email");
            await Expect(loginPage.PasswordError).ToHaveTextAsync("Password must be at least 6 characters");
        }
        finally
        {
            await BrowserFixture.StopTracingAsync(context);
        }
    }

    [Trait("Type", "Smoke")]
    [Fact]
    public async Task ValidLogin()
    {
        await using var context = await _fixture.CreateContextAsync();

        try
        {
            var user = await AuthHelper.RegisterUserAsync(_apiFactory);
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
        finally
        {
            await BrowserFixture.StopTracingAsync(context);
        }
    }

    [Trait("Type", "Smoke")]
    [Fact]
    public async Task Logout()
    {
        await using var context = await _fixture.CreateContextAsync();

        try
        {
            // Arrange
            await AuthHelper.RegisterAndAuthenticateAsync(context, _apiFactory);

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
        finally
        {
            await BrowserFixture.StopTracingAsync(context);
        }
    }
}
