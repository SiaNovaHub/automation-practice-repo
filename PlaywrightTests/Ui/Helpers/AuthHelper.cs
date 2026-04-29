using Microsoft.Playwright;
using PlaywrightTests.Api.Clients;

namespace PlaywrightTests.Ui.Helpers;

public static class AuthHelper
{
    public static async Task LoginViaApiAsync(
        IBrowserContext context,
        ApiClientFactory apiFactory,
        string email,
        string password)
    {
        var apiContext = await apiFactory.CreateContextAsync();
        var authApi = new AuthApi(apiContext);

        var token = await authApi.LoginAsync(email, password);

        await context.AddInitScriptAsync($@"
            localStorage.setItem('token', '{token}');
        ");
    }
}