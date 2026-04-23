using Microsoft.EntityFrameworkCore;
using TourCrm.Core.Entities;
using TourCrm.Core.Entities.Dictionaries;
using TourCrm.Core.Interfaces.Dictionaries;
using TourCrm.Infrastructure.Data;

namespace TourCrm.Infrastructure.Repositories.Dictionaries;

public sealed class CountryRepository(TourCrmDbContext db) : ICountryRepository
{
    public async Task<IReadOnlyList<Country>> GetAllAsync(CancellationToken ct)
        => await db.Countries.AsNoTracking().OrderBy(x => x.Name).ToListAsync(ct);

    public Task<Country?> GetByIdAsync(int id, CancellationToken ct)
        => db.Countries.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<Country> AddAsync(Country e, CancellationToken ct)
    {
        db.Countries.Add(e);
        await db.SaveChangesAsync(ct);
        return e;
    }

    public async Task UpdateAsync(Country e, CancellationToken ct)
    {
        db.Countries.Update(e);
        await db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct)
    {
        var entity = await db.Countries.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null) return;
        db.Countries.Remove(entity);
        await db.SaveChangesAsync(ct);
    }
}