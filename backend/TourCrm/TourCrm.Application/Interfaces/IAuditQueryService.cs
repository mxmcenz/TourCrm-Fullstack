using TourCrm.Application.DTOs;
using TourCrm.Application.DTOs.Auths;

namespace TourCrm.Application.Interfaces;

public interface IAuditQueryService
{
    Task<(IReadOnlyList<AuditLogDto> items, int total)> GetByEntityAsync(
        int companyId, string entity, string entityId, int page, int pageSize, CancellationToken ct);
}