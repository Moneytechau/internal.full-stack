using Backend.Entities;
using Backend.Models.IncidentReports;
using Backend.Repositories;

namespace Backend.Services;

public class IncidentReportService(IIncidentReportRepository repository) : IIncidentReportService
{
    public async Task<IncidentReportResponse> StartAsync(StartIncidentReportRequest request, CancellationToken ct)
    {
        var now = DateTime.UtcNow;

        var entity = new IncidentReportEntity
        {
            Id = Guid.NewGuid(),
            FullName = request.FullName,
            Mobile = request.Mobile,
            CreatedAtUtc = now,
            UpdatedAtUtc = now,
        };

        await repository.AddAsync(entity, ct);

        return ToResponse(entity);
    }

    public async Task<IncidentReportResponse?> UpdateReporterDetailsAsync(Guid id, StartIncidentReportRequest request, CancellationToken ct)
    {
        var entity = await repository.GetByIdAsync(id, ct);
        if (entity is null)
        {
            return null;
        }

        entity.FullName = request.FullName;
        entity.Mobile = request.Mobile;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await repository.SaveChangesAsync(ct);

        return ToResponse(entity);
    }

    public async Task<IncidentReportResponse?> UpdateIncidentAsync(Guid id, UpdateIncidentRequest request, CancellationToken ct)
    {
        var entity = await repository.GetByIdAsync(id, ct);
        if (entity is null)
        {
            return null;
        }

        entity.IncidentType = request.IncidentType;
        entity.EstimatedDamage = request.EstimatedDamage;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await repository.SaveChangesAsync(ct);

        return ToResponse(entity);
    }

    public async Task<IncidentReportResponse?> UpdateDetailsAsync(Guid id, UpdateIncidentDetailsRequest request, CancellationToken ct)
    {
        var entity = await repository.GetByIdAsync(id, ct);
        if (entity is null)
        {
            return null;
        }

        entity.IncidentDate = request.IncidentDate;
        entity.Location = request.Location;
        entity.Description = request.Description;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await repository.SaveChangesAsync(ct);

        return ToResponse(entity);
    }

    private static IncidentReportResponse ToResponse(IncidentReportEntity entity)
    {
        return new IncidentReportResponse
        {
            Id = entity.Id,
            FullName = entity.FullName,
            Mobile = entity.Mobile,
            IncidentType = entity.IncidentType,
            EstimatedDamage = entity.EstimatedDamage,
            IncidentDate = entity.IncidentDate,
            Location = entity.Location,
            Description = entity.Description,
        };
    }
}
