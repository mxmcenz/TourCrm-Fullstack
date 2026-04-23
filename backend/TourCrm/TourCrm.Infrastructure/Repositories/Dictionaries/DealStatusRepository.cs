using Microsoft.EntityFrameworkCore;
using TourCrm.Core.Entities.Dictionaries;
using TourCrm.Core.Interfaces.Dictionaries;
using TourCrm.Infrastructure.Data;

namespace TourCrm.Infrastructure.Repositories.Dictionaries;

public sealed class DealStatusRepository(TourCrmDbContext db) : IDealStatusRepository
{
    public Task<List<DealStatus>> GetAllAsync(CancellationToken ct = default)
        => db.DealStatuses.AsNoTracking()
            .OrderBy(x => x.Name)  
            .ToListAsync(ct);

    public async Task<DealStatus?> GetByIdAsync(int id, CancellationToken ct = default)
        => await db.DealStatuses.FindAsync([id], ct);

    public async Task AddAsync(DealStatus entity, CancellationToken ct = default)
        => await db.DealStatuses.AddAsync(entity, ct);

    public void Update(DealStatus entity) => db.DealStatuses.Update(entity);

    public void Delete(DealStatus entity) => db.DealStatuses.Remove(entity);
    public Task<DealStatus?> GetDefaultAsync(CancellationToken ct = default)
        => db.DealStatuses
            .Where(s => !s.IsFinal)
            .OrderBy(s => s.Id)
            .FirstOrDefaultAsync(ct);
}