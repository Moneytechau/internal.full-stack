namespace Backend.IntegrationTests;

/// <summary>
/// Provides an HttpClient for the already-running API. Does not start or manage the app process.
/// </summary>
public class ApiClientFixture : IAsyncLifetime
{
    public HttpClient Client { get; } = new() { BaseAddress = TestSettings.ApiBaseUrl };

    public async Task InitializeAsync()
    {
        try
        {
            var response = await Client.GetAsync("/health");
            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"API at {TestSettings.ApiBaseUrl} returned {(int)response.StatusCode} from /health.");
            }
        }
        catch (HttpRequestException ex)
        {
            throw new InvalidOperationException(
                $"Could not reach the API at {TestSettings.ApiBaseUrl}. Start it first (e.g. `dotnet run` in backend/), " +
                "or set INTEGRATION_TESTS_API_BASE_URL to point at a running instance.", ex);
        }
    }

    public Task DisposeAsync()
    {
        Client.Dispose();
        return Task.CompletedTask;
    }
}
