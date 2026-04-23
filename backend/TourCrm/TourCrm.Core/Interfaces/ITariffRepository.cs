using TourCrm.Core.Entities.Tariffs;

namespace TourCrm.Core.Interfaces;

public interface ITariffRepository : IGenericRepository<Tariff>
{
    Task<Tariff?> GetWithPermissionsAsync(int id, CancellationToken ct = default);
    Task<Tariff?> GetByNameAsync(string name, CancellationToken ct = default);
    Task<bool> NameExistsAsync(string name, int? excludeId = null, CancellationToken ct = default);
    Task<Tariff?> FindByEmployeeCountAsync(int employees, CancellationToken ct = default);
    Task<IReadOnlyList<Tariff>> GetAllLightAsync(CancellationToken ct = default);
}