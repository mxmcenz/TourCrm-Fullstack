using TourCrm.Core.Entities;
using TourCrm.Core.Entities.Dictionaries;

namespace TourCrm.Core.Interfaces.Dictionaries;

public interface ILeadRequestTypeRepository
{
    Task<IReadOnlyList<LeadRequestType>> GetAllAsync(CancellationToken ct);
    Task<LeadRequestType?> GetByIdAsync(int id, CancellationToken ct);
    Task<LeadRequestType> AddAsync(LeadRequestType e, CancellationToken ct);
    Task UpdateAsync(LeadRequestType e, CancellationToken ct);
    Task DeleteAsync(int id, CancellationToken ct);
}