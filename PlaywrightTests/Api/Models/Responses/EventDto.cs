namespace PlaywrightTests.Api.Models.Responses;

public class EventDto
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string Category { get; set; } = "";
    public string Venue { get; set; } = "";
    public string City { get; set; } = "";
    public DateTime EventDate { get; set; }
    public int Price { get; set; }
    public int TotalSeats { get; set; }
    public int AvailableSeats { get; set; }
    public string? ImageUrl { get; set; }
}
