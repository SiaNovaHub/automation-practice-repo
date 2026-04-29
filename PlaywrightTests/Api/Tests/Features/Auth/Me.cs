using PlaywrightTests.Api.Tests.Infrastructure.Contexts;

namespace PlaywrightTests.Api.Tests.Features.Auth;

public class MeTests(ApiClientFactory factory) : IClassFixture<ApiClientFactory>
{
    private readonly ApiClientFactory _factory = factory;

    private async Task<AuthTestContext> CreateContextAsync()
    {
        var request = await _factory.CreateContextAsync();
        return new AuthTestContext(request);
    }

    [Fact]
    public async Task GetCurrentUser_WithValidToken_Returns200()
    {
        await using var context = await CreateContextAsync();

        var email = $"test_{Guid.NewGuid()}@test.com";
        var password = "Password123!";

        // Arrange
        var user = await context.Auth.RegisterUserAsync(email, password);

        // Act
        var response = await context.Auth.GetCurrentUserRawAsync(user.Token);

        // Assert status
        Assert.Equal(200, response.Status);

        // Assert body
        var json = await response.JsonAsync();
        var root = json.Value;
        var currentUser = root.GetProperty("user");

        Assert.True(root.GetProperty("success").GetBoolean());
        Assert.False(string.IsNullOrEmpty(currentUser.GetProperty("userId").GetInt32().ToString()));
        Assert.Equal(email, currentUser.GetProperty("email").GetString());
    }

    [Fact]
    public async Task GetCurrentUser_WithoutToken_Returns401()
    {
        await using var context = await CreateContextAsync();

        // Act
        var response = await context.Request.GetAsync("/api/auth/me");

        // Assert status
        Assert.Equal(401, response.Status);

        // Assert body
        var json = await response.JsonAsync();
        var root = json.Value;

        Assert.False(root.GetProperty("success").GetBoolean());
        Assert.Equal("Unauthorized", root.GetProperty("error").GetString());
    }

    [Fact]
    public async Task GetCurrentUser_WithInvalidToken_Returns401()
    {
        await using var context = await CreateContextAsync();

        var token = "invalid-token";

        // Act
        var response = await context.Auth.GetCurrentUserRawAsync(token);

        // Assert status
        Assert.Equal(401, response.Status);

        // Assert body
        var json = await response.JsonAsync();
        var root = json.Value;

        Assert.False(root.GetProperty("success").GetBoolean());
        Assert.Equal("Invalid or expired token", root.GetProperty("error").GetString());
    }
}
