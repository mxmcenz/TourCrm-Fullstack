using Microsoft.EntityFrameworkCore;
using TourCrm.Core.Entities;
using TourCrm.Core.Entities.Dictionaries;
using TourCrm.Core.Interfaces.Dictionaries;
using TourCrm.Infrastructure.Data;

namespace TourCrm.Infrastructure.Repositories.Dictionaries;

public sealed class LeadStatusRepository(TourCrmDbContext db) : ILeadStatusRepository
{
    public async Task<IReadOnlyList<LeadStatus>> GetAllAsync(CancellationToken ct)
        => await db.LeadStatuses.AsNoTracking().OrderBy(x => x.Name).ToListAsync(ct);

    public Task<LeadStatus?> GetByIdAsync(int id, CancellationToken ct)
        => db.LeadStatuses.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<LeadStatus> AddAsync(LeadStatus e, CancellationToken ct)
    {
        db.LeadStatuses.Add(e);
        await db.SaveChangesAsync(ct);
        return e;
    }

    public async Task UpdateAsync(LeadStatus e, CancellationToken ct)
    {
        var existing = await db.LeadStatuses.FirstOrDefaultAsync(x => x.Id == e.Id, ct);
        if (existing is null) return;
        existing.Name = e.Name;
        await db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct)
    {
        var entity = await db.LeadStatuses.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null) return;
        db.LeadStatuses.Remove(entity);
        await db.SaveChangesAsync(ct);
    }
}