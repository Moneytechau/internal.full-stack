namespace Backend.IntegrationTests;

/// <summary>
/// Points the tests at an already-running instance of the API and its SQLite database.
/// The API is expected to be started separately (e.g. `dotnet run` in backend/) before running these tests.
/// Override either value with an environment variable when the app isn't running with the defaults below.
/// </summary>
public static class TestSettings
{
    public static Uri ApiBaseUrl { get; } = new(
        Environment.GetEnvironmentVariable("INTEGRATION_TESTS_API_BASE_URL") ?? "http://localhost:5227");

    public static string DatabasePath { get; } =
        Environment.GetEnvironmentVariable("INTEGRATION_TESTS_DB_PATH")
        ?? Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "backend", "incidents.db"));
}
