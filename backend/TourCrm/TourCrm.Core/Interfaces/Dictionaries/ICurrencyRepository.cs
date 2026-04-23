using TourCrm.Core.Entities.Dictionaries;

namespace TourCrm.Core.Interfaces.Dictionaries;

public interface ICurrencyRepository
{
    Task<IReadOnlyList<Currency>> GetAllAsync(CancellationToken ct);
    Task<Currency?> GetByIdAsync(int id, CancellationToken ct);
    Task<Currency> AddAsync(Currency e, CancellationToken ct);
    Task UpdateAsync(Currency e, CancellationToken ct);
    Task DeleteAsync(int id, CancellationToken ct);
}