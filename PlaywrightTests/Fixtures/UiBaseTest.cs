using Microsoft.Playwright;

namespace PlaywrightTests.Fixtures;

public abstract class UiBaseTest(
    BrowserFixture fixture,
    ApiClientFactory apiClientFactory
    ) : IAsyncLifetime
{
    protected readonly BrowserFixture BrowserFixture = fixture;
    protected readonly ApiClientFactory ApiClientFactory = apiClientFactory;

    protected IBrowserContext Context = default!;

    public async Task InitializeAsync()
    {
        Context = await BrowserFixture.CreateContextAsync();
    }

    public async Task DisposeAsync()
    {
        Directory.CreateDirectory("traces");

        await Context.Tracing.StopAsync(new()
        {
            Path = $"traces/{GetType().Name}-{Guid.NewGuid()}.zip"
        });

        await Context.CloseAsync();
    }
}