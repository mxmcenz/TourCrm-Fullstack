using TourCrm.Core.Entities;
using TourCrm.Core.Entities.Dictionaries;

namespace TourCrm.Core.Interfaces.Dictionaries;

public interface ICountryRepository
{
    Task<IReadOnlyList<Country>> GetAllAsync(CancellationToken ct);
    Task<Country?> GetByIdAsync(int id, CancellationToken ct);
    Task<Country> AddAsync(Country e, CancellationToken ct);
    Task UpdateAsync(Country e, CancellationToken ct);
    Task DeleteAsync(int id, CancellationToken ct);
}