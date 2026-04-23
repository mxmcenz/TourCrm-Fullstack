using TourCrm.Core.Entities;
using TourCrm.Core.Entities.Dictionaries;

namespace TourCrm.Core.Interfaces.Dictionaries;

public interface ILeadStatusRepository
{
    Task<IReadOnlyList<LeadStatus>> GetAllAsync(CancellationToken ct);
    Task<LeadStatus?> GetByIdAsync(int id, CancellationToken ct);
    Task<LeadStatus> AddAsync(LeadStatus e, CancellationToken ct);
    Task UpdateAsync(LeadStatus e, CancellationToken ct);
    Task DeleteAsync(int id, CancellationToken ct);
}