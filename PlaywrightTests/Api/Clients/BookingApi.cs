using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Playwright;
using PlaywrightTests.Api.Models.Queries;
using PlaywrightTests.Api.Models.Requests;
using PlaywrightTests.Api.Models.Responses;

namespace PlaywrightTests.Api.Clients;

public class BookingApi(IAPIRequestContext request)
{
    private readonly IAPIRequestContext _request = request;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        NumberHandling = JsonNumberHandling.AllowReadingFromString
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

    private static string BuildQuery(BookingQuery query)
    {
        var parameters = new List<string>();

        if (query.EventId.HasValue)
            parameters.Add($"eventId={query.EventId}");

        if (!string.IsNullOrWhiteSpace(query.Status))
            parameters.Add($"status={Uri.EscapeDataString(query.Status)}");

        if (query.Page.HasValue)
            parameters.Add($"page={query.Page}");

        if (query.Limit.HasValue)
            parameters.Add($"limit={query.Limit}");

        return parameters.Count > 0
            ? "?" + string.Join("&", parameters)
            : "";
    }

    public async Task<IAPIResponse> ListRawAsync(string token, BookingQuery? query = null)
    {
        var queryString = query != null ? BuildQuery(query) : "";

        return await _request.GetAsync($"/api/bookings{queryString}", new()
        {
            Headers = CreateHeaders(token)
        });
    }

    public async Task<BookingListResponse> ListBookingsAsync(string token, BookingQuery? query = null)
    {
        var response = await ListRawAsync(token, query);

        if (!response.Ok)
        {
            var body = await response.TextAsync();
            throw new Exception($"List bookings failed: {response.Status}, {body}");
        }

        return await DeserializeAsync<BookingListResponse>(response);
    }

    public async Task<IAPIResponse> CreateRawAsync(string token, CreateBookingRequest input)
    {
        return await _request.PostAsync("/api/bookings", new()
        {
            Headers = CreateHeaders(token),
            DataObject = input
        });
    }

    public async Task<BookingResponse> CreateBookingResponseAsync(string token, CreateBookingRequest input)
    {
        var response = await CreateRawAsync(token, input);

        if (!response.Ok)
        {
            var body = await response.TextAsync();
            throw new Exception($"Create booking failed: {response.Status}, {body}");
        }

        return await DeserializeAsync<BookingResponse>(response);
    }

    public async Task<BookingDto> CreateBookingAsync(string token, CreateBookingRequest input)
    {
        var bookingResponse = await CreateBookingResponseAsync(token, input);
        return bookingResponse.Data;
    }

    public async Task<IAPIResponse> GetRawAsync(string token, int id)
    {
        return await _request.GetAsync($"/api/bookings/{id}", new()
        {
            Headers = CreateHeaders(token)
        });
    }

    public async Task<BookingResponse> GetBookingResponseAsync(string token, int id)
    {
        var response = await GetRawAsync(token, id);

        if (!response.Ok)
        {
            var body = await response.TextAsync();
            throw new Exception($"Get booking failed: {response.Status}, {body}");
        }

        return await DeserializeAsync<BookingResponse>(response);
    }

    public async Task<BookingDto> GetBookingAsync(string token, int id)
    {
        var bookingResponse = await GetBookingResponseAsync(token, id);
        return bookingResponse.Data;
    }

    public async Task<IAPIResponse> GetByReferenceRawAsync(string token, string reference)
    {
        return await _request.GetAsync($"/api/bookings/ref/{Uri.EscapeDataString(reference)}", new()
        {
            Headers = CreateHeaders(token)
        });
    }

    public async Task<BookingResponse> GetBookingByReferenceResponseAsync(string token, string reference)
    {
        var response = await GetByReferenceRawAsync(token, reference);

        if (!response.Ok)
        {
            var body = await response.TextAsync();
            throw new Exception($"Get booking by reference failed: {response.Status}, {body}");
        }

        return await DeserializeAsync<BookingResponse>(response);
    }

    public async Task<BookingDto> GetBookingByReferenceAsync(string token, string reference)
    {
        var bookingResponse = await GetBookingByReferenceResponseAsync(token, reference);
        return bookingResponse.Data;
    }

    public async Task<IAPIResponse> DeleteRawAsync(string token, int id)
    {
        return await _request.DeleteAsync($"/api/bookings/{id}", new()
        {
            Headers = CreateHeaders(token)
        });
    }
}
