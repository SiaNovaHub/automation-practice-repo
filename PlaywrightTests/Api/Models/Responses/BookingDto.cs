namespace PlaywrightTests.Api.Models.Responses;

public class BookingDto
{
    public int Id { get; set; }
    public int EventId { get; set; }
    public int UserId { get; set; }
    public string CustomerName { get; set; } = "";
    public string CustomerEmail { get; set; } = "";
    public string CustomerPhone { get; set; } = "";
    public int Quantity { get; set; }
    public int TotalPrice { get; set; }
    public string Status { get; set; } = "";
    public string BookingRef { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public EventDto Event { get; set; } = new();
}
