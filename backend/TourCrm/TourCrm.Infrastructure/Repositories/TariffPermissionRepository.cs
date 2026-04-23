using Microsoft.EntityFrameworkCore;
using TourCrm.Core.Entities.Tariffs;
using TourCrm.Core.Interfaces;
using TourCrm.Infrastructure.Data;

namespace TourCrm.Infrastructure.Repositories;

public class TariffPermissionRepository(TourCrmDbContext context)
    : GenericRepository<TariffPermission>(context), ITariffPermissionRepository
{
    private readonly TourCrmDbContext _ctx = context;

    public async Task<IReadOnlyList<TariffPermission>> GetByTariffIdAsync(int tariffId, CancellationToken ct = default)
        => await _ctx.TariffPermissions
            .Where(p => p.TariffId == tariffId)
            .AsNoTracking()
            .ToListAsync(ct);

    public async Task ReplaceForTariffAsync(int tariffId, IEnumerable<TariffPermission> items, CancellationToken ct = default)
    {
        var existing = _ctx.TariffPermissions.Where(p => p.TariffId == tariffId);
        _ctx.TariffPermissions.RemoveRange(existing);
        await _ctx.TariffPermissions.AddRangeAsync(items, ct);
    }
}