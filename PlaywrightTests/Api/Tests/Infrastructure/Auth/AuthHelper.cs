using PlaywrightTests.Api.Clients;

namespace PlaywrightTests.Api.Tests.Infrastructure.Auth;

public static class AuthHelper
{
    public static async Task<string> CreateUserTokenAsync(AuthApi authApi)
    {
        var email = $"test_{Guid.NewGuid()}@test.com";
        var password = "Password123!";

        var user = await authApi.RegisterUserAsync(email, password);
        return user.Token;
    }
}