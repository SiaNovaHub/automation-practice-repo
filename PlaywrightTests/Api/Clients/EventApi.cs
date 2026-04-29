using System.Text.Json;
using Microsoft.Playwright;
using PlaywrightTests.Api.Models.Responses;
using PlaywrightTests.Api.Models.Requests;
using PlaywrightTests.Api.Models.Queries;

namespace PlaywrightTests.Api.Clients;

public class EventApi(IAPIRequestContext request)
{
    private readonly IAPIRequestContext _request = request;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private static Dictionary<string, string> CreateHeaders(string token)
    {
        return new Dictionary<string, string>
        {
            ["Authorization"] = $"Bearer {token}"
        };
    }

    private static async Task<T> DeserializeAsync<T>(IAPIResponse response)
    {
        var body = await response.TextAsync();
        return JsonSerializer.Deserialize<T>(body, JsonOptions)
            ?? throw new Exception($"Failed to deserialize {typeof(T).Name} from response: {body}");
    }

    private static string BuildQuery(EventQuery query)
    {
        var parameters = new List<string>();

        if (!string.IsNullOrWhiteSpace(query.Category))
            parameters.Add($"category={query.Category}");

        if (!string.IsNullOrWhiteSpace(query.City))
            parameters.Add($"city={query.City}");

        if (!string.IsNullOrWhiteSpace(query.Search))
            parameters.Add($"search={query.Search}");

        if (query.Page.HasValue)
            parameters.Add($"page={query.Page}");

        if (query.Limit.HasValue)
            parameters.Add($"limit={query.Limit}");

        return parameters.Count > 0
            ? "?" + string.Join("&", parameters)
            : "";
    }

    public async Task<EventListResponse> ListEventsAsync(string token, EventQuery? query = null)
    {
        var queryString = query != null ? BuildQuery(query) : "";

        var response = await _request.GetAsync($"/api/events{queryString}", new()
        {
            Headers = CreateHeaders(token)
        });

        if (!response.Ok)
        {
            var body = await response.TextAsync();
            throw new Exception($"List events failed: {response.Status}, {body}");
        }

        return await DeserializeAsync<EventListResponse>(response);
    }

    public async Task<IAPIResponse> CreateRawEventAsync(string token, UpsertEventRequest input)
    {
        return await _request.PostAsync("/api/events", new()
        {
            Headers = CreateHeaders(token),
            DataObject = input
        });
    }

    public async Task<EventResponse> CreateEventResponseAsync(string token, UpsertEventRequest input)
    {
        var response = await CreateRawEventAsync(token, input);

        if (!response.Ok)
        {
            var body = await response.TextAsync();
            throw new Exception($"Create event failed: {response.Status}, {body}");
        }

        return await DeserializeAsync<EventResponse>(response);
    }

    public async Task<EventDto> CreateEventAsync(string token, UpsertEventRequest input)
    {
        var eventResponse = await CreateEventResponseAsync(token, input);
        return eventResponse.Data;
    }

    public async Task<IAPIResponse> GetRawAsync(string token, int id)
    {
        return await _request.GetAsync($"/api/events/{id}", new()
        {
            Headers = CreateHeaders(token)
        });
    }

    public async Task<EventResponse> GetEventResponseAsync(string token, int id)
    {
        var response = await GetRawAsync(token, id);

        if (!response.Ok)
        {
            var body = await response.TextAsync();
            throw new Exception($"Get event failed: {response.Status}, {body}");
        }

        return await DeserializeAsync<EventResponse>(response);
    }

    public async Task<EventDto> GetEventAsync(string token, int id)
    {
        var eventResponse = await GetEventResponseAsync(token, id);
        return eventResponse.Data;
    }

    public async Task<IAPIResponse> UpdateRawAsync(string token, int id, UpsertEventRequest input)
    {
        return await _request.PutAsync($"/api/events/{id}", new()
        {
            Headers = CreateHeaders(token),
            DataObject = input
        });
    }

    public async Task<EventResponse> UpdateEventAsync(string token, int id, UpsertEventRequest input)
    {
        var response = await UpdateRawAsync(token, id, input);

        if (!response.Ok)
        {
            var body = await response.TextAsync();
            throw new Exception($"Update event failed: {response.Status}, {body}");
        }

        return await DeserializeAsync<EventResponse>(response);
    }

    public async Task<IAPIResponse> DeleteRawAsync(string token, int id)
    {
        return await _request.DeleteAsync($"/api/events/{id}", new()
        {
            Headers = CreateHeaders(token)
        });
    }
}
