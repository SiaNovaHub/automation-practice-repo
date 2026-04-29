using Microsoft.Playwright;
using PlaywrightTests.Api.Clients;

namespace PlaywrightTests.Ui.Helpers;

public static class AuthHelper
{
    public static Task AuthenticateWithTokenAsync(IBrowserContext context, string token)
    {
        return context.AddInitScriptAsync($@"
            localStorage.setItem('eventhub_token', '{token}');
        ");
    }

    public static async Task<UiUser> RegisterUserAsync(ApiClientFactory apiFactory)
    {
        await using var apiContext = await apiFactory.CreateContextAsync();
        var authApi = new AuthApi(apiContext);

        var user = new UiUser
        {
            Email = $"ui_{Guid.NewGuid()}@test.com",
            Password = "Password123!"
        };

        var registeredUser = await authApi.RegisterUserAsync(user.Email, user.Password);
        user.Token = registeredUser.Token;

        return user;
    }

    public static async Task<UiUser> RegisterAndAuthenticateAsync(IBrowserContext context, ApiClientFactory apiFactory)
    {
        var user = await RegisterUserAsync(apiFactory);
        await AuthenticateWithTokenAsync(context, user.Token);
        return user;
    }

    public static async Task LoginViaApiAsync(
        IBrowserContext context,
        ApiClientFactory apiFactory,
        string email,
        string password)
    {
        await using var apiContext = await apiFactory.CreateContextAsync();
        var authApi = new AuthApi(apiContext);

        var token = await authApi.LoginAsync(email, password);

        await AuthenticateWithTokenAsync(context, token);
    }
}
