using Microsoft.EntityFrameworkCore;
using TourCrm.Core.Entities;
using TourCrm.Core.Entities.Leads;
using TourCrm.Core.Interfaces;
using TourCrm.Infrastructure.Data;

namespace TourCrm.Infrastructure.Repositories;

public sealed class LeadSelectionRepository(TourCrmDbContext ctx)
    : GenericRepository<LeadSelection>(ctx), ILeadSelectionRepository
{
    public async Task<LeadSelection?> GetAsync(int leadId, int id, CancellationToken ct = default) =>
        await _dbSet.AsNoTracking()
            .FirstOrDefaultAsync(s => s.LeadId == leadId && s.Id == id, ct);
    
    public async Task<IReadOnlyList<LeadSelection>> ListByLeadAsync(int leadId, CancellationToken ct = default) =>
        await _dbSet.AsNoTracking()
            .Where(s => s.LeadId == leadId)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync(ct);
    public async Task<LeadSelection?> GetByLeadForUpdateAsync(int leadId, CancellationToken ct = default) =>
        await _dbSet
            .Where(s => s.LeadId == leadId)
            .OrderByDescending(s => s.CreatedAt)
            .FirstOrDefaultAsync(ct);
    public async Task<LeadSelection?> GetLastByLeadAsync(int leadId, CancellationToken ct = default) =>
        await _dbSet.AsNoTracking()
            .Where(s => s.LeadId == leadId)
            .OrderByDescending(s => s.CreatedAt)
            .FirstOrDefaultAsync(ct);
    public async Task<LeadSelection?> GetForUpdateAsync(int leadId, int id, CancellationToken ct = default) =>
        await _dbSet.FirstOrDefaultAsync(s => s.LeadId == leadId && s.Id == id, ct);
}