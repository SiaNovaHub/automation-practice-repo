using PlaywrightTests.Api.Tests.Infrastructure.Contexts;
using PlaywrightTests.Fixtures;

namespace PlaywrightTests.Api.Tests.Features.Auth;

[Trait("Layer", "API")]
[Trait("Feature", "Auth")]
public class RegisterTests(ApiClientFactory factory) : IClassFixture<ApiClientFactory>
{
    private readonly ApiClientFactory _factory = factory;
    private async Task<AuthTestContext> CreateContextAsync()
    {
        var request = await _factory.CreateContextAsync();
        return new AuthTestContext(request);
    }

    [Trait("Type", "Smoke")]
    [Fact]
    public async Task Register_WithValidData_ReturnsCreatedUser()
    {
        await using var context = await CreateContextAsync();

        var email = $"test_{Guid.NewGuid()}@test.com";
        var password = "Password123!";

        // Act
        var user = await context.Auth.RegisterUserAsync(email, password);

        // Assert
        Assert.False(string.IsNullOrEmpty(user.Id));
        Assert.Equal(email, user.Email);
        Assert.False(string.IsNullOrEmpty(user.Token));
    }

    [Trait("Type", "Regression")]
    [Fact]
    public async Task Register_WithEmptyEmail_Returns400()
    {
        await using var context = await CreateContextAsync();

        var password = "Password123!";
        var email = "";

        // Act
        var response = await context.Auth.RegisterRawUserAsync(email, password);

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

    [Trait("Type", "Regression")]
    [Fact]
    public async Task Register_WithInvalidEmail_Returns400()
    {
        await using var context = await CreateContextAsync();

        var email = "invalid-email";
        var password = "Password123!";

        // Act
        var response = await context.Auth.RegisterRawUserAsync(email, password);

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

    [Trait("Type", "Regression")]
    [Fact]
    public async Task Register_WithShortPassword_Returns400()
    {
        await using var context = await CreateContextAsync();

        var email = $"test_{Guid.NewGuid()}@test.com";
        var password = "12345";

        // Act
        var response = await context.Auth.RegisterRawUserAsync(email, password);

        // Assert status
        Assert.Equal(400, response.Status);

        // Assert body
        var json = await response.JsonAsync();
        var root = json.Value;
        var error = root.GetProperty("details")[0];

        Assert.False(root.GetProperty("success").GetBoolean());
        Assert.Equal("Validation failed", root.GetProperty("error").GetString());
        Assert.Equal("password", error.GetProperty("field").GetString());
        Assert.Equal("Password must be at least 6 characters", error.GetProperty("message").GetString());
    }

    [Trait("Type", "Regression")]
    [Fact]
    public async Task Register_WithExistingEmail_Returns400()
    {
        await using var context = await CreateContextAsync();

        var email = $"test_{Guid.NewGuid()}@test.com";
        var password = "Password123!";

        // Arrange
        await context.Auth.RegisterUserAsync(email, password);

        // Act
        var response = await context.Auth.RegisterRawUserAsync(email, password);

        // Assert status
        Assert.Equal(400, response.Status);

        // Assert body
        var json = await response.JsonAsync();
        var root = json.Value;

        Assert.False(root.GetProperty("success").GetBoolean());
        Assert.Equal("Email already registered", root.GetProperty("error").GetString());
        Assert.Equal(0, root.GetProperty("details").GetArrayLength());
    }
}
