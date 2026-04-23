using Microsoft.EntityFrameworkCore;
using TourCrm.Core.Entities.Dictionaries;
using TourCrm.Core.Interfaces.Dictionaries;
using TourCrm.Infrastructure.Data;

namespace TourCrm.Infrastructure.Repositories.Dictionaries;

public sealed class HotelRepository(TourCrmDbContext db) : IHotelRepository
{
    public async Task<IReadOnlyList<Hotel>> GetAllAsync(CancellationToken ct)
        => await db.Hotels.AsNoTracking().OrderBy(x => x.Name).ToListAsync(ct);

    public Task<Hotel?> GetByIdAsync(int id, CancellationToken ct)
        => db.Hotels.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<Hotel> AddAsync(Hotel e, CancellationToken ct)
    {
        db.Hotels.Add(e);
        await db.SaveChangesAsync(ct);
        return e;
    }

    public async Task UpdateAsync(Hotel e, CancellationToken ct)
    {
        var existing = await db.Hotels.FirstOrDefaultAsync(x => x.Id == e.Id, ct);
        if (existing is null) return;
        existing.Name  = e.Name;
        existing.Stars = e.Stars;
        existing.CityId = e.CityId;
        await db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct)
    {
        var entity = await db.Hotels.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null) return;
        db.Hotels.Remove(entity);
        await db.SaveChangesAsync(ct);
    }
}