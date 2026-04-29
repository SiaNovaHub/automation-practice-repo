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

    [Fact]
    public async Task Login_WithWrongPassword_Returns400()
    {
        await using var context = await CreateContextAsync();

        var email = $"test_{Guid.NewGuid()}@test.com";
        var password = "Password123!";
        var wrongPassword = "WrongPassword123!";

        // Arrange
        await context.Auth.RegisterUserAsync(email, password);

        // Act
        var response = await context.Auth.LoginRawAsync(email, wrongPassword);

        // Assert status
        Assert.Equal(400, response.Status);

        // Assert body
        var json = await response.JsonAsync();
        var root = json.Value;

        Assert.False(root.GetProperty("success").GetBoolean());
        Assert.Equal("Invalid email or password", root.GetProperty("error").GetString());
        Assert.Equal(0, root.GetProperty("details").GetArrayLength());
    }

    [Fact]
    public async Task Login_WithUnknownEmail_Returns404()
    {
        await using var context = await CreateContextAsync();

        var email = $"missing_{Guid.NewGuid()}@test.com";
        var password = "Password123!";

        // Act
        var response = await context.Auth.LoginRawAsync(email, password);

        // Assert status
        Assert.Equal(404, response.Status);

        // Assert body
        var json = await response.JsonAsync();
        var root = json.Value;

        Assert.False(root.GetProperty("success").GetBoolean());
        Assert.Equal("User not found", root.GetProperty("error").GetString());
    }

    [Fact]
    public async Task Login_WithEmptyEmail_Returns400()
    {
        await using var context = await CreateContextAsync();

        var email = "";
        var password = "Password123!";

        // Act
        var response = await context.Auth.LoginRawAsync(email, password);

        // Assert status
        Assert.Equal(400, response.Status);

        // Assert body
        var json = await response.JsonAsync();
        var root = json.Value;
        var error = root.GetProperty("details")[0];

        Assert.False(root.GetProperty("success").GetBoolean());
        Assert.Equal("Validation failed", root.GetProperty("error").GetString());
        Assert.Equal("email", error.GetProperty("field").GetString());
        Assert.Equal("A valid email is required", error.GetProperty("message").GetString());
    }
}
