using TourCrm.Core.Entities;
using TourCrm.Core.Entities.Deals;

namespace TourCrm.Core.Interfaces;

public interface IDealHistoryRepository : IGenericRepository<DealHistory>
{
    Task<List<DealHistory>> GetByDealAsync(int dealId, CancellationToken ct = default);
}