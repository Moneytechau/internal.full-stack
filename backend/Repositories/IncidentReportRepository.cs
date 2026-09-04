using Backend.Data;
using Backend.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories;

public class IncidentReportRepository(AppDbContext db) : IIncidentReportRepository
{
    public async Task<IncidentReportEntity> AddAsync(IncidentReportEntity entity, CancellationToken ct)
    {
        db.IncidentReports.Add(entity);
        await db.SaveChangesAsync(ct);
        return entity;
    }

    public Task<IncidentReportEntity?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return db.IncidentReports.FirstOrDefaultAsync(r => r.Id == id, ct);
    }

    public Task SaveChangesAsync(CancellationToken ct)
    {
        return db.SaveChangesAsync(ct);
    }
}
