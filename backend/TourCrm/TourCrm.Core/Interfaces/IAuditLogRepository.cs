using TourCrm.Core.Entities;
using TourCrm.Core.Enums;

namespace TourCrm.Core.Interfaces;

public interface IAuditLogRepository : IGenericRepository<AuditLog>
{
    Task AddAsync(int companyId, string entity, string entityId, AuditAction action, string dataJson, int? userId,
        CancellationToken ct = default);

    Task<IReadOnlyList<AuditLog>> GetByEntityAsync(int companyId, string entity, string entityId, int page,
        int pageSize, CancellationToken ct = default);

    Task<int> CountByEntityAsync(int companyId, string entity, string entityId, CancellationToken ct = default);
}