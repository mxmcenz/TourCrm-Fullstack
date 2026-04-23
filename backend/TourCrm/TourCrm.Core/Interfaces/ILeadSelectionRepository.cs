using TourCrm.Core.Entities;
using TourCrm.Core.Entities.Leads;

namespace TourCrm.Core.Interfaces;

public interface ILeadSelectionRepository : IGenericRepository<LeadSelection>
{
    Task<LeadSelection?> GetAsync(int leadId, int id, CancellationToken ct = default);
    Task<IReadOnlyList<LeadSelection>> ListByLeadAsync(int leadId, CancellationToken ct = default);
    Task<LeadSelection?> GetByLeadForUpdateAsync(int leadId, CancellationToken ct = default);
    Task<LeadSelection?> GetLastByLeadAsync(int leadId, CancellationToken ct = default);
    Task<LeadSelection?> GetForUpdateAsync(int leadId, int id, CancellationToken ct = default);
}