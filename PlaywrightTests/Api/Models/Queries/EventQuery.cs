namespace PlaywrightTests.Api.Models.Queries;
public class EventQuery
{
    public string? Category { get; set; }
    public string? City { get; set; }
    public string? Search { get; set; }
    public int? Page { get; set; }
    public int? Limit { get; set; }
}