namespace PlaywrightTests.Api.Models.Queries;

public class BookingQuery
{
    public int? EventId { get; set; }
    public string? Status { get; set; }
    public int? Page { get; set; }
    public int? Limit { get; set; }
}
