using Microsoft.Playwright;
using PlaywrightTests.Api.Clients;

namespace PlaywrightTests.Api.Tests.Infrastructure.Contexts;

public class BookingTestContext(IAPIRequestContext request) : IAsyncDisposable
{
    public IAPIRequestContext Request { get; } = request;
    public AuthApi Auth { get; } = new AuthApi(request);
    public EventApi Events { get; } = new EventApi(request);
    public BookingApi Bookings { get; } = new BookingApi(request);

    public async ValueTask DisposeAsync()
    {
        await Request.DisposeAsync();
    }
}
