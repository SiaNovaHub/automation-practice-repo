namespace PlaywrightTests.Config;

public class PlaywrightSettings
{
    public string BaseUrl { get; set; } = "";
    public string ApiBaseUrl { get; set; } = "";
    public string Browser { get; set; } = "chromium";
    public LaunchOptions LaunchOptions { get; set; } = new();
    public ContextOptions ContextOptions { get; set; } = new();
    public int Timeout { get; set; } = 30000;
}

public class LaunchOptions
{
    public bool Headless { get; set; }
    public float? SlowMo { get; set; }
    public string[]? Args { get; set; }
}

public class ContextOptions
{
    public bool UseViewport { get; set; } = true;
    public int ViewportWidth { get; set; }
    public int ViewportHeight { get; set; }
}