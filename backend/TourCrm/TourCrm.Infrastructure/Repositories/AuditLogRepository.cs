using Microsoft.EntityFrameworkCore;
using TourCrm.Core.Entities;
using TourCrm.Core.Enums;
using TourCrm.Core.Interfaces;
using TourCrm.Infrastructure.Data;

namespace TourCrm.Infrastructure.Repositories;

public class AuditLogRepository(TourCrmDbContext context) : GenericRepository<AuditLog>(context), IAuditLogRepository
{
    public async Task AddAsync(int companyId, string entity, string entityId, AuditAction action, string dataJson,
        int? userId, CancellationToken ct = default)
    {
        var log = new AuditLog
        {
            CompanyId = companyId,
            Entity = entity,
            EntityId = entityId,
            Action = action,
            DataJson = dataJson,
            UserId = userId,
            AtUtc = DateTime.UtcNow
        };
        await _dbSet.AddAsync(log, ct);
    }

    public async Task<IReadOnlyList<AuditLog>> GetByEntityAsync(int companyId, string entity, string entityId, int page,
        int pageSize, CancellationToken ct = default)
        => await _dbSet.AsNoTracking()
            .Where(x => x.CompanyId == companyId && x.Entity == entity && x.EntityId == entityId)
            .OrderByDescending(x => x.AtUtc)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .ToListAsync(ct);

    public async Task<int> CountByEntityAsync(int companyId, string entity, string entityId,
        CancellationToken ct = default)
        => await _dbSet.CountAsync(x => x.CompanyId == companyId && x.Entity == entity && x.EntityId == entityId, ct);
}