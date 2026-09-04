using Microsoft.Playwright;

namespace FunctionalTests;

/// <summary>
/// Owns the Playwright driver and a browser instance shared across tests in a class.
/// Does not start or manage the app under test — that's expected to already be running.
/// </summary>
public class PlaywrightFixture : IAsyncLifetime
{
    private IPlaywright _playwright = null!;

    public IBrowser Browser { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        _playwright = await Playwright.CreateAsync();
        Browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = TestSettings.Headless,
        });
    }

    public async Task DisposeAsync()
    {
        await Browser.CloseAsync();
        _playwright.Dispose();
    }
}
