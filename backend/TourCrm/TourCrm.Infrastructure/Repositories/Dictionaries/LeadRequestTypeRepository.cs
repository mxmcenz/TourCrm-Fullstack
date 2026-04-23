using Microsoft.EntityFrameworkCore;
using TourCrm.Core.Entities;
using TourCrm.Core.Entities.Dictionaries;
using TourCrm.Core.Interfaces.Dictionaries;
using TourCrm.Infrastructure.Data;

namespace TourCrm.Infrastructure.Repositories.Dictionaries;

public sealed class LeadRequestTypeRepository(TourCrmDbContext db) : ILeadRequestTypeRepository
{
    public async Task<IReadOnlyList<LeadRequestType>> GetAllAsync(CancellationToken ct)
        => await db.LeadRequestTypes.AsNoTracking().OrderBy(x => x.Name).ToListAsync(ct);

    public Task<LeadRequestType?> GetByIdAsync(int id, CancellationToken ct)
        => db.LeadRequestTypes.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<LeadRequestType> AddAsync(LeadRequestType e, CancellationToken ct)
    {
        db.LeadRequestTypes.Add(e);
        await db.SaveChangesAsync(ct);
        return e;
    }

    public async Task UpdateAsync(LeadRequestType e, CancellationToken ct)
    {
        var existing = await db.LeadRequestTypes.FirstOrDefaultAsync(x => x.Id == e.Id, ct);
        if (existing is null) return;
        existing.Name = e.Name;
        await db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct)
    {
        var entity = await db.LeadRequestTypes.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null) return;
        db.LeadRequestTypes.Remove(entity);
        await db.SaveChangesAsync(ct);
    }
}