using System.Text.Json;
using TourCrm.Application.DTOs;
using TourCrm.Application.DTOs.Auths;
using TourCrm.Application.Interfaces;
using TourCrm.Core.Interfaces;

namespace TourCrm.Application.Services;

public sealed class AuditQueryService(IUnitOfWork uow) : IAuditQueryService
{
    public async Task<(IReadOnlyList<AuditLogDto> items, int total)> GetByEntityAsync(
        int companyId, string entity, string entityId, int page, int pageSize, CancellationToken ct)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;

        var items = await uow.AuditLogs.GetByEntityAsync(companyId, entity, entityId, page, pageSize, ct);
        var total = await uow.AuditLogs.CountByEntityAsync(companyId, entity, entityId, ct);

        var dtos = items.Select(x => new AuditLogDto
        {
            Id = x.Id,
            CompanyId = x.CompanyId,
            Entity = x.Entity,
            EntityId = x.EntityId,
            Action = x.Action.ToString(),
            Data = JsonDocument.Parse(x.DataJson).RootElement.Clone(),
            UserId = x.UserId,
            AtUtc = x.AtUtc
        }).ToList();

        return (dtos, total);
    }
}