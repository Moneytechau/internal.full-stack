using Backend.Entities;

namespace Backend.Repositories;

public interface IIncidentReportRepository
{
    Task<IncidentReportEntity> AddAsync(IncidentReportEntity entity, CancellationToken ct);

    Task<IncidentReportEntity?> GetByIdAsync(Guid id, CancellationToken ct);

    Task SaveChangesAsync(CancellationToken ct);
}
