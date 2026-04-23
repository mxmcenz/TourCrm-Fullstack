using TourCrm.Core.Entities.Tariffs;

namespace TourCrm.Core.Interfaces;

public interface ITariffPermissionRepository : IGenericRepository<TariffPermission>
{
    Task<IReadOnlyList<TariffPermission>> GetByTariffIdAsync(int tariffId, CancellationToken ct = default);
    Task ReplaceForTariffAsync(int tariffId, IEnumerable<TariffPermission> items, CancellationToken ct = default);
}