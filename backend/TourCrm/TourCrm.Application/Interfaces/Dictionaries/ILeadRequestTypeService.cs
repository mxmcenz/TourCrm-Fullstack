using TourCrm.Application.DTOs.Dictionaries.LeadRequestTypes;

namespace TourCrm.Application.Interfaces.Dictionaries;

public interface ILeadRequestTypeService
{
    Task<IReadOnlyList<LeadRequestTypeDto>> GetAllAsync(CancellationToken ct);
    Task<LeadRequestTypeDto> CreateAsync(CreateLeadRequestTypeDto dto, CancellationToken ct);
    Task UpdateAsync(int id, UpdateLeadRequestTypeDto dto, CancellationToken ct);
    Task DeleteAsync(int id, CancellationToken ct);
}