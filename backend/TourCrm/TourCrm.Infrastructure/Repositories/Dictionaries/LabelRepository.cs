using Microsoft.EntityFrameworkCore;
using TourCrm.Core.Entities;
using TourCrm.Core.Entities.Dictionaries;
using TourCrm.Core.Interfaces.Dictionaries;
using TourCrm.Infrastructure.Data;

namespace TourCrm.Infrastructure.Repositories.Dictionaries;

public sealed class LabelRepository(TourCrmDbContext db) : ILabelRepository
{
    public async Task<IReadOnlyList<Label>> GetAllAsync(CancellationToken ct)
        => await db.Labels.AsNoTracking().OrderBy(x => x.Name).ToListAsync(ct);

    public Task<Label?> GetByIdAsync(int id, CancellationToken ct)
        => db.Labels.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<Label> AddAsync(Label e, CancellationToken ct)
    {
        db.Labels.Add(e);
        await db.SaveChangesAsync(ct);
        return e;
    }

    public async Task UpdateAsync(Label e, CancellationToken ct)
    {
        var existing = await db.Labels.FirstOrDefaultAsync(x => x.Id == e.Id, ct);
        if (existing is null) return;
        existing.Name = e.Name; 
        await db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct)
    {
        var entity = await db.Labels.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null) return;
        db.Labels.Remove(entity);
        await db.SaveChangesAsync(ct);
    }
}