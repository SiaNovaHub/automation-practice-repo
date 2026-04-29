namespace PlaywrightTests.Api.Models.Responses;

public class BookingResponse
{
    public bool Success { get; set; }
    public BookingDto Data { get; set; } = new();
    public string? Message { get; set; }
}

public class BookingListResponse
{
    public bool Success { get; set; }
    public List<BookingDto> Data { get; set; } = new();
    public PaginationMeta Pagination { get; set; } = new();
}
