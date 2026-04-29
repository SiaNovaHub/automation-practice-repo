namespace PlaywrightTests.Api.Models.Responses;

public class EventResponse
{
    public bool Success { get; set; }
    public EventDto Data { get; set; } = new();
    public string? Message { get; set; }
}

public class EventListResponse
{
    public bool Success { get; set; }
    public List<EventDto> Data { get; set; } = new();
    public PaginationMeta Pagination { get; set; } = new();
}

public class PaginationMeta
{
    public int Total { get; set; }
    public int Page { get; set; }
    public int Limit { get; set; }
    public int TotalPages { get; set; }
}
