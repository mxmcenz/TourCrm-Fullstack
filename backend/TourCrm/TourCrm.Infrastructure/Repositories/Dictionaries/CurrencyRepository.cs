using Microsoft.EntityFrameworkCore;
using TourCrm.Core.Entities.Dictionaries;
using TourCrm.Core.Interfaces.Dictionaries;
using TourCrm.Infrastructure.Data;

namespace TourCrm.Infrastructure.Repositories.Dictionaries;

public sealed class CurrencyRepository(TourCrmDbContext db) : ICurrencyRepository
{
    public async Task<IReadOnlyList<Currency>> GetAllAsync(CancellationToken ct)
        => await db.Currencies.AsNoTracking().OrderBy(x => x.Name).ToListAsync(ct);

    public Task<Currency?> GetByIdAsync(int id, CancellationToken ct)
        => db.Currencies.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<Currency> AddAsync(Currency e, CancellationToken ct)
    {
        db.Currencies.Add(e);
        await db.SaveChangesAsync(ct);
        return e;
    }

    public async Task UpdateAsync(Currency e, CancellationToken ct)
    {
        var existing = await db.Currencies.FirstOrDefaultAsync(x => x.Id == e.Id, ct);
        if (existing is null) return;
        existing.Name = e.Name;
        await db.SaveChangesAsync(ct);
    }


    public async Task DeleteAsync(int id, CancellationToken ct)
    {
        var entity = await db.Currencies.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null) return;
        db.Currencies.Remove(entity);
        await db.SaveChangesAsync(ct);
    }
}