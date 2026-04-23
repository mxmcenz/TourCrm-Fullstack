using Microsoft.EntityFrameworkCore;
using TourCrm.Core.Entities;
using TourCrm.Core.Entities.Leads;
using TourCrm.Core.Interfaces;
using TourCrm.Infrastructure.Data;

namespace TourCrm.Infrastructure.Repositories;

public sealed class LeadHistoryRepository(TourCrmDbContext context)
    : GenericRepository<LeadHistory>(context), ILeadHistoryRepository
{
    public new async Task AddAsync(LeadHistory entity, CancellationToken ct = default)
        => await base.AddAsync(entity, ct);

    public async Task<IReadOnlyList<LeadHistory>> ListAsync(int leadId, string userId, CancellationToken ct = default)
        => await _dbSet.AsNoTracking()
            .Where(h => h.LeadId == leadId)
            .OrderByDescending(h => h.CreatedAt)
            .ToListAsync(ct);
}