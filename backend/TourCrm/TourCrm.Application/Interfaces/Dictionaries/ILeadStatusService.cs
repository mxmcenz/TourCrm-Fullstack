using TourCrm.Application.DTOs.Dictionaries.LeadStatuses;

namespace TourCrm.Application.Interfaces.Dictionaries;

public interface ILeadStatusService
{
    Task<IReadOnlyList<LeadStatusDto>> GetAllAsync(CancellationToken ct);
    Task<LeadStatusDto> CreateAsync(CreateLeadStatusDto dto, CancellationToken ct);
    Task UpdateAsync(int id, UpdateLeadStatusDto dto, CancellationToken ct);
    Task DeleteAsync(int id, CancellationToken ct);
}