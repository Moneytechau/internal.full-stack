using Microsoft.Playwright;

namespace FunctionalTests;

public class IncidentReportFlowTests : IClassFixture<PlaywrightFixture>
{
    private readonly PlaywrightFixture _fixture;

    public IncidentReportFlowTests(PlaywrightFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task ReportIncident_EndToEnd_CompletesAllStepsAndShowsConfirmation()
    {
        var page = await _fixture.Browser.NewPageAsync();

        try
        {
            await page.GotoAsync(TestSettings.FrontendBaseUrl);

            // Step 1 — reporter details
            await page.GetByLabel("Full name").FillAsync("Jane Doe");
            await page.GetByLabel("Mobile number").FillAsync("0412345678");
            await page.GetByRole(AriaRole.Button, new() { Name = "Continue" }).ClickAsync();

            // Step 2 — what happened
            await page.GetByLabel("Incident type").SelectOptionAsync("Theft");
            await page.GetByLabel("Estimated damage").FillAsync("1500");
            await page.GetByRole(AriaRole.Button, new() { Name = "Continue" }).ClickAsync();

            // Step 3 — when and where
            await page.GetByLabel("Date of incident").FillAsync("2026-01-15");
            await page.GetByLabel("Location").FillAsync("Sydney");
            await page.GetByLabel("Description").FillAsync("Vehicle collision at the intersection of Main St and King St.");
            await page.GetByRole(AriaRole.Button, new() { Name = "Submit report" }).ClickAsync();

            await Assertions.Expect(page.GetByText("Thanks — your report has been submitted")).ToBeVisibleAsync();
        }
        finally
        {
            await page.CloseAsync();
        }
    }
}
