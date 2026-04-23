using Microsoft.EntityFrameworkCore;
using TourCrm.Core.Entities;
using TourCrm.Core.Entities.Dictionaries;
using TourCrm.Core.Interfaces.Dictionaries;
using TourCrm.Infrastructure.Data;

namespace TourCrm.Infrastructure.Repositories.Dictionaries;

public sealed class LeadSourceRepository(TourCrmDbContext db) : ILeadSourceRepository
{
    public async Task<IReadOnlyList<LeadSource>> GetAllAsync(CancellationToken ct)
        => await db.LeadSources.AsNoTracking().OrderBy(x => x.Name).ToListAsync(ct);

    public Task<LeadSource?> GetByIdAsync(int id, CancellationToken ct)
        => db.LeadSources.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<LeadSource> AddAsync(LeadSource e, CancellationToken ct)
    {
        db.LeadSources.Add(e);
        await db.SaveChangesAsync(ct);
        return e;
    }

    public async Task UpdateAsync(LeadSource e, CancellationToken ct)
    {
        var existing = await db.LeadSources.FirstOrDefaultAsync(x => x.Id == e.Id, ct);
        if (existing is null) return;
        existing.Name = e.Name;
        await db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct)
    {
        var entity = await db.LeadSources.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null) return;
        db.LeadSources.Remove(entity);
        await db.SaveChangesAsync(ct);
    }
}