using Microsoft.EntityFrameworkCore;
using TourCrm.Core.Entities;
using TourCrm.Core.Entities.Deals;
using TourCrm.Core.Interfaces;
using TourCrm.Infrastructure.Data;

namespace TourCrm.Infrastructure.Repositories;

public class DealHistoryRepository(TourCrmDbContext db)
    : GenericRepository<DealHistory>(db), IDealHistoryRepository
{
    public Task<List<DealHistory>> GetByDealAsync(int dealId, CancellationToken ct = default) =>
        _context.Set<DealHistory>()
            .Where(x => x.DealId == dealId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(ct);
}