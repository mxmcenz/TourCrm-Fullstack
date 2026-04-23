using TourCrm.Core.Entities.Dictionaries;

namespace TourCrm.Core.Interfaces.Dictionaries;

public interface IDealStatusRepository
{
    Task<List<DealStatus>> GetAllAsync(CancellationToken ct = default);
    Task<DealStatus?> GetByIdAsync(int id, CancellationToken ct = default);
    Task AddAsync(DealStatus entity, CancellationToken ct = default);
    void Update(DealStatus entity);
    void Delete(DealStatus entity);
    Task<DealStatus?> GetDefaultAsync(CancellationToken ct = default);
}