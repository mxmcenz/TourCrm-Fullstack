using TourCrm.Core.Entities;
using TourCrm.Core.Entities.Deals;
using TourCrm.Core.Specifications;

namespace TourCrm.Core.Interfaces;

public interface IDealRepository : IGenericRepository<Deal>
{
    Task<List<Deal>> GetAllWithIncludesAsync(
        int? statusId = null, int? officeId = null, int? managerId = null,
        CancellationToken ct = default);

    Task<Deal?> GetByIdWithIncludesAsync(int id, CancellationToken ct = default);

    Task ChangeStatusAsync(int dealId, int statusId, CancellationToken ct = default);
    Task<PagedResult<Deal>> SearchAsync(DealSearchRequest req, CancellationToken ct = default);
    Task<Deal?> GetByIdIgnoringFiltersAsync(int id, CancellationToken ct = default);
}