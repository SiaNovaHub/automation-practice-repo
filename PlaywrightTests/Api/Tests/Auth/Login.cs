namespace PlaywrightTests.Api.Tests.Auth;

public class LoginTests(ApiClientFactory factory) : IClassFixture<ApiClientFactory>
{
    private readonly ApiClientFactory _factory = factory;
    private async Task<AuthTestContext> CreateContextAsync()
    {
        var request = await _factory.CreateContextAsync();
        return new AuthTestContext(request);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsValidToken()
    {
        await using var context = await CreateContextAsync();

        var email = $"test_{Guid.NewGuid()}@test.com";
        var password = "Password123!";

        // Arrange
        await context.Auth.RegisterUserAsync(email, password);

        // Act
        var token = await context.Auth.LoginAsync(email, password);

        // Assert
        Assert.False(string.IsNullOrEmpty(token));
    }
}
