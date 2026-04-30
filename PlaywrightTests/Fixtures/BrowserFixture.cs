using Microsoft.Playwright;
using PlaywrightTests.Config;

namespace PlaywrightTests.Fixtures;

public class BrowserFixture : IAsyncLifetime
{
    public IBrowser Browser { get; private set; } = default!;

    private IPlaywright _playwright = default!;
    private PlaywrightSettings _settings = default!;

    public async Task InitializeAsync()
    {
        _settings = ConfigLoader.Load();

        _playwright = await Playwright.CreateAsync();

        var launchOptions = new BrowserTypeLaunchOptions
        {
            Headless = _settings.LaunchOptions.Headless,
            SlowMo = _settings.LaunchOptions.SlowMo,
            Args = _settings.LaunchOptions.Args
        };

        var browsers = new Dictionary<string, Func<IBrowserType>>
        {
            ["chromium"] = () => _playwright.Chromium,
            ["firefox"] = () => _playwright.Firefox,
            ["webkit"] = () => _playwright.Webkit
        };

        var browserType = browsers[_settings.Browser.ToLower()]();

        Browser = await browserType.LaunchAsync(launchOptions);
    }

    public async Task<IBrowserContext> CreateContextAsync()
    {
        var context = await Browser.NewContextAsync(new BrowserNewContextOptions
        {
            BaseURL = _settings.BaseUrl,
            ViewportSize = _settings.ContextOptions.UseViewport
                ? new()
                {
                    Width = _settings.ContextOptions.ViewportWidth,
                    Height = _settings.ContextOptions.ViewportHeight
                }
                : null
        });

        await context.Tracing.StartAsync(new()
        {
            Screenshots = true,
            Snapshots = true,
            Sources = true
        });

        context.SetDefaultTimeout(_settings.Timeout);

        return context;
    }

    public async Task DisposeAsync()
    {
        await Browser.CloseAsync();
        _playwright.Dispose();
    }
}