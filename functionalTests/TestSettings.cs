namespace FunctionalTests;

/// <summary>
/// Points the tests at an already-running instance of the frontend (and, transitively, the API it talks to).
/// Both are expected to be started separately (e.g. `ng serve` and `dotnet run`) before running these tests.
/// </summary>
public static class TestSettings
{
    public static string FrontendBaseUrl { get; } =
        Environment.GetEnvironmentVariable("FUNCTIONAL_TESTS_FRONTEND_BASE_URL") ?? "http://localhost:4200";

    public static bool Headless { get; } =
        Environment.GetEnvironmentVariable("FUNCTIONAL_TESTS_HEADLESS") is not ("false" or "0");
}
