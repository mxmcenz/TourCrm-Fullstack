using Microsoft.EntityFrameworkCore;
using TourCrm.Core.Entities.Dictionaries;
using TourCrm.Core.Interfaces.Dictionaries;
using TourCrm.Infrastructure.Data;

namespace TourCrm.Infrastructure.Repositories.Dictionaries;

public sealed class VisaTypeRepository(TourCrmDbContext db) : IVisaTypeRepository
{
    public async Task<IReadOnlyList<VisaType>> GetAllAsync(CancellationToken ct)
        => await db.VisaTypes.AsNoTracking().OrderBy(x => x.Name).ToListAsync(ct);

    public Task<VisaType?> GetByIdAsync(int id, CancellationToken ct)
        => db.VisaTypes.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<VisaType> AddAsync(VisaType e, CancellationToken ct)
    {
        db.VisaTypes.Add(e);
        await db.SaveChangesAsync(ct);
        return e;
    }

    public async Task UpdateAsync(VisaType e, CancellationToken ct)
    {
        var existing = await db.VisaTypes.FirstOrDefaultAsync(x => x.Id == e.Id, ct);
        if (existing is null) return;
        existing.Name = e.Name;
        await db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct)
    {
        var entity = await db.VisaTypes.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null) return;
        db.VisaTypes.Remove(entity);
        await db.SaveChangesAsync(ct);
    }
}