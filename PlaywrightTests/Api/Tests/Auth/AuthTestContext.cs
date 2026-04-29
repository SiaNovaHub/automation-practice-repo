using Microsoft.Playwright;
using PlaywrightTests.Api.Clients;

namespace PlaywrightTests.Api.Tests.Auth;

public class AuthTestContext(IAPIRequestContext request) : IAsyncDisposable
{
    public IAPIRequestContext Request { get; } = request;
    public AuthApi Auth { get; } = new AuthApi(request);

    public async ValueTask DisposeAsync()
    {
        await Request.DisposeAsync();
    }
}