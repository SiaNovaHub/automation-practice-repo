namespace PlaywrightTests.Fixtures;

[CollectionDefinition("UI collection")]
public class UiCollection 
    : ICollectionFixture<BrowserFixture>, 
      ICollectionFixture<ApiClientFactory>
{
}