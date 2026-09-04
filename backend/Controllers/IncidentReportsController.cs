using Backend.Models.IncidentReports;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IncidentReportsController(IIncidentReportService incidentReportService) : ControllerBase
{
    /// <summary>
    /// Step 1 — starts a new incident report with the reporter's details.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<IncidentReportResponse>> Start(StartIncidentReportRequest request, CancellationToken ct)
    {
        var report = await incidentReportService.StartAsync(request, ct);
        return CreatedAtAction(nameof(Start), new { id = report.Id }, report);
    }

    /// <summary>
    /// Step 2 — saves the incident type and estimated damage.
    /// </summary>
    [HttpPut("{id:guid}/incident")]
    public async Task<ActionResult<IncidentReportResponse>> UpdateIncident(Guid id, UpdateIncidentRequest request, CancellationToken ct)
    {
        var report = await incidentReportService.UpdateIncidentAsync(id, request, ct);
        return report is null ? NotFound() : Ok(report);
    }

    /// <summary>
    /// Step 3 — saves the date, location and description of the incident.
    /// </summary>
    [HttpPut("{id:guid}/details")]
    public async Task<ActionResult<IncidentReportResponse>> UpdateDetails(Guid id, UpdateIncidentDetailsRequest request, CancellationToken ct)
    {
        var report = await incidentReportService.UpdateDetailsAsync(id, request, ct);
        return report is null ? NotFound() : Ok(report);
    }
}
