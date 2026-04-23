using TourCrm.Application.DTOs.Leads;

namespace TourCrm.Application.Interfaces;

public interface ILeadService
{
    Task<IEnumerable<LeadDto>> GetAllAsync(CancellationToken ct = default);
    Task<LeadDto?> GetByIdAsync(int id, CancellationToken ct = default);

    Task<LeadDto> CreateAsync(CreateLeadDto dto, CancellationToken ct = default);
    Task UpdateAsync(int id, UpdateLeadDto dto, CancellationToken ct = default);
    Task AssignUserAsync(int id, int userId, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);

    Task<IEnumerable<LeadDto>> FilterByStatusAsync(string statusName, CancellationToken ct = default);

    Task<LeadPageDto> SearchAsync(LeadFilterDto filter, CancellationToken ct = default);
    Task<IReadOnlyList<LeadHistoryDto>> GetHistoryAsync(int leadId, CancellationToken ct = default);
}