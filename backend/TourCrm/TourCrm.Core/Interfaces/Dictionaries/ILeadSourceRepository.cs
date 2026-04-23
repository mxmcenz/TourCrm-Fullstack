using TourCrm.Core.Entities;
using TourCrm.Core.Entities.Dictionaries;

namespace TourCrm.Core.Interfaces.Dictionaries;

public interface ILeadSourceRepository
{
    Task<IReadOnlyList<LeadSource>> GetAllAsync(CancellationToken ct);
    Task<LeadSource?> GetByIdAsync(int id, CancellationToken ct);
    Task<LeadSource> AddAsync(LeadSource e, CancellationToken ct);
    Task UpdateAsync(LeadSource e, CancellationToken ct);
    Task DeleteAsync(int id, CancellationToken ct);
}