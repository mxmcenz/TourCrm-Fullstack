using TourCrm.Core.Entities;
using TourCrm.Core.Entities.Leads;

namespace TourCrm.Core.Interfaces;

public interface ILeadHistoryRepository : IGenericRepository<LeadHistory>
{
    Task AddAsync(LeadHistory history, CancellationToken ct = default); 
    Task<IReadOnlyList<LeadHistory>> ListAsync(int leadId, string userId, CancellationToken ct = default);
}