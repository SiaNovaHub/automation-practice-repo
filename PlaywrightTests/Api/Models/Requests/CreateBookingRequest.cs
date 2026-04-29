namespace PlaywrightTests.Api.Models.Requests;

public class CreateBookingRequest
{
    public int EventId { get; set; }
    public string CustomerName { get; set; } = "";
    public string CustomerEmail { get; set; } = "";
    public string CustomerPhone { get; set; } = "";
    public int Quantity { get; set; }
}
