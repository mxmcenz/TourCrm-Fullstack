using TourCrm.Core.Entities;

namespace TourCrm.Core.Interfaces;

public interface ICompanyRepository : IGenericRepository<Company>
{
    Task<Company?> GetByOwnerAsync(string ownerUserId, CancellationToken ct = default);
    Task<bool> ExistsForOwnerAsync(string ownerUserId, CancellationToken ct = default);
    Task<Company?> GetByLegalEntityIdAsync(int legalEntityId, CancellationToken ct = default);
}