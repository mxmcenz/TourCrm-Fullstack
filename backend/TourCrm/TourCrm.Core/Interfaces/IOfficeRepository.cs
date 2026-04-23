using TourCrm.Core.Entities;

namespace TourCrm.Core.Interfaces;

public interface IOfficeRepository : IGenericRepository<Office>
{
    Task<bool> ExistsNameInLegalAsync(int legalEntityId, string name, CancellationToken ct = default);
    Task<Office?> GetByIdAsync(int id, bool includeLegal = false, bool includeEmployees = false, CancellationToken ct = default);
    Task<IReadOnlyList<Office>> GetByLegalAsync(int legalEntityId, bool includeLegal = false, bool includeEmployees = false, CancellationToken ct = default);
}