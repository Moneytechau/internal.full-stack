using System.Net;
using System.Net.Http.Json;
using Backend.Models.IncidentReports;
using Microsoft.Data.Sqlite;

namespace Backend.IntegrationTests;

public class IncidentReportsControllerTests : IClassFixture<ApiClientFixture>
{
    private readonly HttpClient _client;

    public IncidentReportsControllerTests(ApiClientFixture app)
    {
        _client = app.Client;
    }

    [Fact]
    public async Task Start_WithValidRequest_ReturnsCreatedWithReport()
    {
        var request = new StartIncidentReportRequest { FullName = "Jane Doe", Mobile = "0412345678" };

        var response = await _client.PostAsJsonAsync("/api/incidentreports", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);

        var report = await response.Content.ReadFromJsonAsync<IncidentReportResponse>();
        Assert.NotNull(report);
        Assert.NotEqual(Guid.Empty, report!.Id);
        Assert.Equal(request.FullName, report.FullName);
        Assert.Equal(request.Mobile, report.Mobile);
        Assert.Null(report.IncidentType);
    }

    [Fact]
    public async Task Start_WithValidRequest_PersistsRowInDatabase()
    {
        var request = new StartIncidentReportRequest { FullName = "Row Check", Mobile = "0412345678" };

        var response = await _client.PostAsJsonAsync("/api/incidentreports", request);
        var report = await response.Content.ReadFromJsonAsync<IncidentReportResponse>();

        using var connection = new SqliteConnection($"Data Source={TestSettings.DatabasePath}");
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT FullName, Mobile FROM IncidentReports WHERE Id = $id COLLATE NOCASE";
        command.Parameters.AddWithValue("$id", report!.Id.ToString());

        using var reader = command.ExecuteReader();
        Assert.True(reader.Read());
        Assert.Equal(request.FullName, reader.GetString(0));
        Assert.Equal(request.Mobile, reader.GetString(1));
    }

    [Theory]
    [InlineData("", "0412345678")]
    [InlineData("Jane Doe", "not-a-number")]
    public async Task Start_WithInvalidRequest_ReturnsBadRequest(string fullName, string mobile)
    {
        var request = new StartIncidentReportRequest { FullName = fullName, Mobile = mobile };

        var response = await _client.PostAsJsonAsync("/api/incidentreports", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateReporterDetails_WhenReportExists_ReturnsOkWithUpdatedReport()
    {
        var id = await CreateReportAsync();

        var request = new StartIncidentReportRequest { FullName = "Jane Smith", Mobile = "0498765432" };
        var response = await _client.PutAsJsonAsync($"/api/incidentreports/{id}", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var report = await response.Content.ReadFromJsonAsync<IncidentReportResponse>();
        Assert.Equal(request.FullName, report!.FullName);
        Assert.Equal(request.Mobile, report.Mobile);
    }

    [Fact]
    public async Task UpdateReporterDetails_WhenReportDoesNotExist_ReturnsNotFound()
    {
        var request = new StartIncidentReportRequest { FullName = "Jane Smith", Mobile = "0498765432" };

        var response = await _client.PutAsJsonAsync($"/api/incidentreports/{Guid.NewGuid()}", request);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateIncident_WhenReportExists_ReturnsOkWithUpdatedReport()
    {
        var id = await CreateReportAsync();

        var request = new UpdateIncidentRequest { IncidentType = "Theft", EstimatedDamage = 2500m };
        var response = await _client.PutAsJsonAsync($"/api/incidentreports/{id}/incident", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var report = await response.Content.ReadFromJsonAsync<IncidentReportResponse>();
        Assert.Equal(request.IncidentType, report!.IncidentType);
        Assert.Equal(request.EstimatedDamage, report.EstimatedDamage);
    }

    [Fact]
    public async Task UpdateIncident_WhenReportDoesNotExist_ReturnsNotFound()
    {
        var request = new UpdateIncidentRequest { IncidentType = "Theft", EstimatedDamage = 2500m };

        var response = await _client.PutAsJsonAsync($"/api/incidentreports/{Guid.NewGuid()}/incident", request);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateIncident_WithInvalidRequest_ReturnsBadRequest()
    {
        var id = await CreateReportAsync();

        var request = new UpdateIncidentRequest { IncidentType = "Theft", EstimatedDamage = -10m };
        var response = await _client.PutAsJsonAsync($"/api/incidentreports/{id}/incident", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateDetails_WhenReportExists_ReturnsOkWithUpdatedReport()
    {
        var id = await CreateReportAsync();

        var request = new UpdateIncidentDetailsRequest
        {
            IncidentDate = new DateOnly(2026, 1, 15),
            Location = "Sydney",
            Description = "Vehicle collision at intersection.",
        };
        var response = await _client.PutAsJsonAsync($"/api/incidentreports/{id}/details", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var report = await response.Content.ReadFromJsonAsync<IncidentReportResponse>();
        Assert.Equal(request.IncidentDate, report!.IncidentDate);
        Assert.Equal(request.Location, report.Location);
        Assert.Equal(request.Description, report.Description);
    }

    [Fact]
    public async Task UpdateDetails_WhenReportDoesNotExist_ReturnsNotFound()
    {
        var request = new UpdateIncidentDetailsRequest
        {
            IncidentDate = new DateOnly(2026, 1, 15),
            Location = "Sydney",
            Description = "Vehicle collision at intersection.",
        };

        var response = await _client.PutAsJsonAsync($"/api/incidentreports/{Guid.NewGuid()}/details", request);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateDetails_WithInvalidRequest_ReturnsBadRequest()
    {
        var id = await CreateReportAsync();

        var request = new UpdateIncidentDetailsRequest
        {
            IncidentDate = new DateOnly(2026, 1, 15),
            Location = "Sydney",
            Description = "too short",
        };
        var response = await _client.PutAsJsonAsync($"/api/incidentreports/{id}/details", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task FullWorkflow_CreateThenUpdateEachStep_PersistsAllFields()
    {
        var startResponse = await _client.PostAsJsonAsync(
            "/api/incidentreports",
            new StartIncidentReportRequest { FullName = "Alex Chen", Mobile = "0411222333" });
        var started = await startResponse.Content.ReadFromJsonAsync<IncidentReportResponse>();

        var incidentResponse = await _client.PutAsJsonAsync(
            $"/api/incidentreports/{started!.Id}/incident",
            new UpdateIncidentRequest { IncidentType = "Fire", EstimatedDamage = 10000m });
        Assert.Equal(HttpStatusCode.OK, incidentResponse.StatusCode);

        var detailsResponse = await _client.PutAsJsonAsync(
            $"/api/incidentreports/{started.Id}/details",
            new UpdateIncidentDetailsRequest
            {
                IncidentDate = new DateOnly(2026, 2, 1),
                Location = "Melbourne",
                Description = "Kitchen fire caused smoke damage.",
            });

        Assert.Equal(HttpStatusCode.OK, detailsResponse.StatusCode);
        var finalReport = await detailsResponse.Content.ReadFromJsonAsync<IncidentReportResponse>();

        Assert.Equal(started.Id, finalReport!.Id);
        Assert.Equal("Alex Chen", finalReport.FullName);
        Assert.Equal("0411222333", finalReport.Mobile);
        Assert.Equal("Fire", finalReport.IncidentType);
        Assert.Equal(10000m, finalReport.EstimatedDamage);
        Assert.Equal(new DateOnly(2026, 2, 1), finalReport.IncidentDate);
        Assert.Equal("Melbourne", finalReport.Location);
        Assert.Equal("Kitchen fire caused smoke damage.", finalReport.Description);

        using var connection = new SqliteConnection($"Data Source={TestSettings.DatabasePath}");
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT IncidentType, Location FROM IncidentReports WHERE Id = $id COLLATE NOCASE";
        command.Parameters.AddWithValue("$id", started.Id.ToString());

        using var reader = command.ExecuteReader();
        Assert.True(reader.Read());
        Assert.Equal("Fire", reader.GetString(0));
        Assert.Equal("Melbourne", reader.GetString(1));
    }

    [Fact]
    public async Task HealthCheck_ReturnsOk()
    {
        var response = await _client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    private async Task<Guid> CreateReportAsync()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/incidentreports",
            new StartIncidentReportRequest { FullName = "Jane Doe", Mobile = "0412345678" });
        var report = await response.Content.ReadFromJsonAsync<IncidentReportResponse>();
        return report!.Id;
    }
}
