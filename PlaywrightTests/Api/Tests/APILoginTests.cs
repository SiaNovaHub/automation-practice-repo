namespace PlaywrightTests.Api.Tests;

public class APILoginTests(ApiClientFactory factory) : IClassFixture<ApiClientFactory>
{
    private readonly ApiClientFactory _factory = factory;

    [Fact]
    public async Task LoginReturnsValidToken()
    {
        await using var request = await _factory.CreateContextAsync();

        var response = await request.PostAsync("/api/auth/login", new()
        {
            DataObject = new
            {
                email = "testaz@test.com",
                password = "passWord123!"
            }
        });

        Assert.True(response.Ok);

        var json = await response.JsonAsync();
        var token = json.Value.GetProperty("token").GetString();

        Assert.False(string.IsNullOrEmpty(token));
    }

}
