using Backend.Models.IncidentReports;

namespace Backend.Services;

public interface IIncidentReportService
{
    Task<IncidentReportResponse> StartAsync(StartIncidentReportRequest request, CancellationToken ct);

    Task<IncidentReportResponse?> UpdateReporterDetailsAsync(Guid id, StartIncidentReportRequest request, CancellationToken ct);

    Task<IncidentReportResponse?> UpdateIncidentAsync(Guid id, UpdateIncidentRequest request, CancellationToken ct);

    Task<IncidentReportResponse?> UpdateDetailsAsync(Guid id, UpdateIncidentDetailsRequest request, CancellationToken ct);
}
