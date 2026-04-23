using TourCrm.Core.Enums;

namespace TourCrm.Application.Interfaces;

public interface IAuditLogger
{
    Task LogAsync(int companyId, string entity, string entityId, AuditAction action, string dataJson, int? userId,
        CancellationToken ct);
}