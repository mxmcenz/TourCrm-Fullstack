using TourCrm.Application.DTOs.Leads;

namespace TourCrm.Application.Interfaces;

public interface ILeadSelectionService
{
    Task<LeadSelectionDto?> GetAsync(int leadId, int id, string userId, CancellationToken ct = default);
    Task<LeadSelectionDto?> GetSingleByLeadAsync(int leadId, string userId, CancellationToken ct = default);
    Task<LeadSelectionDto> CreateAsync(int leadId, CreateLeadSelectionDto dto, string userId, CancellationToken ct = default);
    Task<LeadSelectionDto> UpdateAsync(int leadId, int id, UpdateLeadSelectionDto dto, string userId, CancellationToken ct = default);
}