namespace PlaywrightTests.Ui.Helpers;

public class UiUser
{
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
    public string Token { get; set; } = "";
}

public class UiEventData
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string Category { get; set; } = "";
    public string Venue { get; set; } = "";
    public string City { get; set; } = "";
    public string EventDate { get; set; } = "";
    public int Price { get; set; }
    public int TotalSeats { get; set; }
    public int AvailableSeats { get; set; }
    public string? ImageUrl { get; set; }
}

public class UiBookingData
{
    public int Id { get; set; }
    public int EventId { get; set; }
    public string CustomerName { get; set; } = "";
    public string CustomerEmail { get; set; } = "";
    public string CustomerPhone { get; set; } = "";
    public int Quantity { get; set; }
    public int TotalPrice { get; set; }
    public string Status { get; set; } = "";
    public string BookingRef { get; set; } = "";
}
