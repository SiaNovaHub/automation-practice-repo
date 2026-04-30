using PlaywrightTests.Config;
using Microsoft.Playwright;

namespace PlaywrightTests.Fixtures;

public class ApiClientFactory : IAsyncLifetime
{
    private IPlaywright _playwright = default!;
    private PlaywrightSettings _settings = default!;

    public async Task InitializeAsync()
    {
        _settings = ConfigLoader.Load();
        _playwright = await Playwright.CreateAsync();
    }

    public async Task DisposeAsync()
    {
        _playwright.Dispose();
    }

    public async Task<IAPIRequestContext> CreateContextAsync()
    {
        return await _playwright.APIRequest.NewContextAsync(new APIRequestNewContextOptions
        {
            BaseURL = _settings.ApiBaseUrl
        });
    }
}