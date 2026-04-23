using TourCrm.Application.DTOs.Dictionaries.LeadSources;

namespace TourCrm.Application.Interfaces.Dictionaries;

public interface ILeadSourceService
{
    Task<IReadOnlyList<LeadSourceDto>> GetAllAsync(CancellationToken ct);
    Task<LeadSourceDto> CreateAsync(CreateLeadSourceDto dto, CancellationToken ct);
    Task UpdateAsync(int id, UpdateLeadSourceDto dto, CancellationToken ct);
    Task DeleteAsync(int id, CancellationToken ct);
}