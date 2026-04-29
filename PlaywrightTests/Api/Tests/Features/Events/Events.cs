using PlaywrightTests.Api.Tests.Infrastructure.Contexts;
using PlaywrightTests.Api.Tests.Infrastructure.Helpers;
using PlaywrightTests.Api.Tests.Infrastructure.Auth;
using PlaywrightTests.Api.Tests.Infrastructure.Builders;

namespace PlaywrightTests.Api.Tests.Features.Events;

public class EventsTests(ApiClientFactory factory) : IClassFixture<ApiClientFactory>
{
    private readonly ApiClientFactory _factory = factory;

    private async Task<EventTestContext> CreateContextAsync()
    {
        var request = await _factory.CreateContextAsync();
        return new EventTestContext(request);
    }

    [Fact]
    public async Task ListEvents_Returns200()
    {
        await using var context = await CreateContextAsync();
        var token = await AuthHelper.CreateUserTokenAsync(context.Auth);

        // Act
        var response = await context.Events.ListEventsAsync(token);

        // Assert status
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.NotNull(response.Pagination);

        // Assert body
        Assert.True(response.Pagination.Page >= 1);
        Assert.True(response.Pagination.Limit >= 1);

        if (response.Data.Count > 0)
        {
            var firstEvent = response.Data[0];

            Assert.True(firstEvent.Id > 0);
            Assert.False(string.IsNullOrEmpty(firstEvent.Title));
        }
    }

    [Fact]
    public async Task CreateEvent_WithValidData_Returns201()
    {
        await using var context = await CreateContextAsync();
        var token = await AuthHelper.CreateUserTokenAsync(context.Auth);
        var input = EventBuilder.Valid();

        // Act
        var response = await context.Events.CreateRawEventAsync(token, input);

        // Assert status
        Assert.Equal(201, response.Status);
    }

    [Fact]
    public async Task CreateEvent_WithValidData_ReturnsValidResponseStructure()
    {
        await using var context = await CreateContextAsync();
        var token = await AuthHelper.CreateUserTokenAsync(context.Auth);
        var input = EventBuilder.Valid();

        // Act
        var response = await context.Events.CreateEventResponseAsync(token, input);

        // Assert response structure
        Assert.True(response.Success);
        Assert.Equal("Event created successfully", response.Message);
        Assert.NotNull(response.Data);
    }

    [Fact]
    public async Task CreateEvent_WithValidData_ReturnsValidResponseData()
    {
        await using var context = await CreateContextAsync();
        var token = await AuthHelper.CreateUserTokenAsync(context.Auth);
        var input = EventBuilder.Valid();

        // Act
        var response = await context.Events.CreateEventResponseAsync(token, input);

        // Assert response data
        Assert.Equal(input.Title, response.Data.Title);
        Assert.Equal(input.TotalSeats, response.Data.TotalSeats);
        Assert.Equal(input.TotalSeats, response.Data.AvailableSeats);
    }

    [Fact]
    public async Task CreateEvent_WithMissingTitle_Returns400()
    {
        await using var context = await CreateContextAsync();
        var token = await AuthHelper.CreateUserTokenAsync(context.Auth);
        var input = EventBuilder.Valid();
        input.Title = "";

        // Act
        var response = await context.Events.CreateRawEventAsync(token, input);

        // Assert status
        Assert.Equal(400, response.Status);

        // Assert body
        var root = await EventTestHelpers.GetRootAsync(response);
        var error = root.GetProperty("details")[0];

        Assert.False(root.GetProperty("success").GetBoolean());
        Assert.Equal("Validation failed", root.GetProperty("error").GetString());
        Assert.Equal("title", error.GetProperty("field").GetString());
        Assert.Equal("Title is required", error.GetProperty("message").GetString());
    }

    [Fact]
    public async Task GetEventById_WithExistingId_Returns200()
    {
        await using var context = await CreateContextAsync();
        var token = await AuthHelper.CreateUserTokenAsync(context.Auth);
        var input = EventBuilder.Valid();
        var id = await EventTestHelpers.CreateTestEventAsync(context.Events, token, input);

        try
        {
            // Act
            var response = await context.Events.GetRawAsync(token, id);

            // Assert status
            Assert.Equal(200, response.Status);

            // Assert body
            var root = await EventTestHelpers.GetRootAsync(response);
            var data = EventTestHelpers.GetData(root);

            Assert.True(root.GetProperty("success").GetBoolean());
            Assert.Equal(id, data.GetProperty("id").GetInt32());
            Assert.Equal(input.Title, data.GetProperty("title").GetString());
        }
        finally
        {
            await context.Events.DeleteRawAsync(token, id);
        }
    }

    [Fact]
    public async Task GetEventById_WithUnknownId_Returns404()
    {
        await using var context = await CreateContextAsync();
        var token = await AuthHelper.CreateUserTokenAsync(context.Auth);
        var eventId = 999999;

        // Act
        var response = await context.Events.GetRawAsync(token, eventId);

        // Assert status
        Assert.Equal(404, response.Status);

        // Assert body
        var root = await EventTestHelpers.GetRootAsync(response);

        Assert.False(root.GetProperty("success").GetBoolean());
        Assert.Equal($"Event with id {eventId} not found", root.GetProperty("error").GetString());
    }

    [Fact]
    public async Task UpdateEvent_WithValidData_Returns200()
    {
        await using var context = await CreateContextAsync();
        var token = await AuthHelper.CreateUserTokenAsync(context.Auth);
        var input = EventBuilder.Valid();
        var id = await EventTestHelpers.CreateTestEventAsync(context.Events, token, input);
        var updatedInput = EventBuilder.Valid();
        updatedInput.Title = $"Updated Event {Guid.NewGuid():N}"[..22];
        updatedInput.Description = "Updated API test event";

        try
        {
            // Act
            var response = await context.Events.UpdateRawAsync(token, id, updatedInput);

            // Assert status
            Assert.Equal(200, response.Status);

            // Assert body
            var root = await EventTestHelpers.GetRootAsync(response);
            var data = EventTestHelpers.GetData(root);

            Assert.True(root.GetProperty("success").GetBoolean());
            Assert.Equal("Event updated successfully", root.GetProperty("message").GetString());
            Assert.Equal(id, data.GetProperty("id").GetInt32());
            Assert.Equal(updatedInput.Title, data.GetProperty("title").GetString());
            Assert.Equal(updatedInput.Description, data.GetProperty("description").GetString());
        }
        finally
        {
            await context.Events.DeleteRawAsync(token, id);
        }
    }

    [Fact]
    public async Task UpdateEvent_WithUnknownId_Returns404()
    {
        await using var context = await CreateContextAsync();
        var token = await AuthHelper.CreateUserTokenAsync(context.Auth);
        var eventId = 999999;
        var input = EventBuilder.Valid();

        // Act
        var response = await context.Events.UpdateRawAsync(token, eventId, input);

        // Assert status
        Assert.Equal(404, response.Status);

        // Assert body
        var root = await EventTestHelpers.GetRootAsync(response);

        Assert.False(root.GetProperty("success").GetBoolean());
        Assert.Equal($"Event with id {eventId} not found", root.GetProperty("error").GetString());
    }

    [Fact]
    public async Task DeleteEvent_WithExistingId_Returns200()
    {
        await using var context = await CreateContextAsync();
        var token = await AuthHelper.CreateUserTokenAsync(context.Auth);
        var input = EventBuilder.Valid();
        var id = await EventTestHelpers.CreateTestEventAsync(context.Events, token, input);

        // Act
        var response = await context.Events.DeleteRawAsync(token, id);

        // Assert status
        Assert.Equal(200, response.Status);

        // Assert body
        var root = await EventTestHelpers.GetRootAsync(response);

        Assert.True(root.GetProperty("success").GetBoolean());
        Assert.Equal("Event deleted successfully", root.GetProperty("message").GetString());
    }

    [Fact]
    public async Task DeleteEvent_WithUnknownId_Returns404()
    {
        await using var context = await CreateContextAsync();
        var token = await AuthHelper.CreateUserTokenAsync(context.Auth);
        var eventId = 999999;

        // Act
        var response = await context.Events.DeleteRawAsync(token, eventId);

        // Assert status
        Assert.Equal(404, response.Status);

        // Assert body
        var root = await EventTestHelpers.GetRootAsync(response);

        Assert.False(root.GetProperty("success").GetBoolean());
        Assert.Equal($"Event with id {eventId} not found", root.GetProperty("error").GetString());
    }
}
