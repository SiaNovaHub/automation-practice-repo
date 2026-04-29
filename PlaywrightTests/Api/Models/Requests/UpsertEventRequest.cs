namespace PlaywrightTests.Api.Models.Requests;

public class UpsertEventRequest
{
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string Category { get; set; } = "";
    public string Venue { get; set; } = "";
    public string City { get; set; } = "";
    public string EventDate { get; set; } = "";
    public int Price { get; set; }
    public int TotalSeats { get; set; }
    public string? ImageUrl { get; set; }
}
