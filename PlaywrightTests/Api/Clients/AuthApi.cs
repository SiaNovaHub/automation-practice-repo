using Microsoft.Playwright;
using PlaywrightTests.Api.Models;

namespace PlaywrightTests.Api.Clients;

public class AuthApi(IAPIRequestContext request)
{
    private readonly IAPIRequestContext _request = request;

    public async Task<AuthUser> RegisterUserAsync(string email, string password)
    {
        var response = await _request.PostAsync("/api/auth/register", new()
        {
            DataObject = new { email, password }
        });

        if (!response.Ok)
        {
            var body = await response.TextAsync();
            throw new Exception($"User creation request failed: {response.Status}, response body: {body}");
        }

        var json = await response.JsonAsync();
        var root = json.Value;
        var user = root.GetProperty("user");

        return new AuthUser
        {
            Id = user.GetProperty("id").GetInt32().ToString(),
            Email = user.GetProperty("email").GetString()!,
            Token = root.GetProperty("token").GetString()!
        };
    }

    public async Task<IAPIResponse> RegisterRawUserAsync(string email, string password)
    {
        return await _request.PostAsync("/api/auth/register", new()
        {
            DataObject = new { email, password }
        });
    }

    public async Task<IAPIResponse> LoginRawAsync(string email, string password)
    {
        return await _request.PostAsync("/api/auth/login", new()
        {
            DataObject = new { email, password }
        });
    }

    public async Task<string> LoginAsync(string email, string password)
    {
        var response = await LoginRawAsync(email, password);

        if (!response.Ok)
        {
            var body = await response.TextAsync();
            throw new Exception($"Login failed: {response.Status}, {body}");
        }

        var json = await response.JsonAsync();
        return json.Value.GetProperty("token").GetString()!;
    }

    public async Task<IAPIResponse> GetCurrentUserRawAsync(string token)
    {
        return await _request.GetAsync("/api/auth/me", new()
        {
            Headers = new Dictionary<string, string>
            {
                ["Authorization"] = $"Bearer {token}"
            }
        });
    }

    public async Task<AuthUser> GetCurrentUserAsync(string token)
    {
        var response = await GetCurrentUserRawAsync(token);

        if (!response.Ok)
        {
            var body = await response.TextAsync();
            throw new Exception($"Get user failed: {response.Status}, {body}");
        }

        var json = await response.JsonAsync();
        var user = json.Value.GetProperty("user");

        return new AuthUser
        {
            Id = user.GetProperty("userId").GetInt32().ToString(),
            Email = user.GetProperty("email").GetString()!,
            Token = token
        };
    }
}
