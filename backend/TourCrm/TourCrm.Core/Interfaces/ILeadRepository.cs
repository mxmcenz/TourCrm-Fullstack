using TourCrm.Core.Entities;
using TourCrm.Core.Entities.Leads;
using TourCrm.Core.Specifications;

namespace TourCrm.Core.Interfaces;

public interface ILeadRepository : IGenericRepository<Lead>
{
    Task<Lead?> GetAsync(int id, string userId, CancellationToken ct = default);
    Task<Lead?> GetByNumberAsync(string leadNumber, string userId, CancellationToken ct = default);
    Task<(IReadOnlyList<Lead> Items, long Total)> SearchAsync(LeadSearchSpec spec, string userId, CancellationToken ct = default);
    Task SoftDeleteAsync(int id, string userId, CancellationToken ct = default);
    Task<Dictionary<int, int>> GetLeadCountsByManagerAsync(int companyId, CancellationToken ct = default);
    Task<int> CountActiveByOfficeAsync(int companyId, int officeId, CancellationToken ct = default);
    Task<Dictionary<int,int>> GetActiveLeadCountsByOfficeAsync(int companyId, CancellationToken ct = default);
}