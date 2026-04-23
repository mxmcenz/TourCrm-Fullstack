using TourCrm.Application.Interfaces;
using TourCrm.Core.Enums;
using TourCrm.Core.Interfaces;

namespace TourCrm.Application.Services;

public class AuditLogger(IUnitOfWork uow) : IAuditLogger
{
    public async Task LogAsync(int companyId, string entity, string entityId, AuditAction action, string dataJson,
        int? userId, CancellationToken ct)
    {
        await uow.AuditLogs.AddAsync(companyId, entity, entityId, action, dataJson, userId, ct);
        await uow.SaveChangesAsync(ct);
    }
}