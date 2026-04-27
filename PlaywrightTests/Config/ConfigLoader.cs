using Microsoft.Extensions.Configuration;

namespace PlaywrightTests.Config;

public static class ConfigLoader
{
    public static PlaywrightSettings Load()
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddEnvironmentVariables()
            .Build();

        var settings = config.GetSection("Playwright").Get<PlaywrightSettings>() ?? throw new Exception("Invalid Playwright config");
        return settings;
    }
}