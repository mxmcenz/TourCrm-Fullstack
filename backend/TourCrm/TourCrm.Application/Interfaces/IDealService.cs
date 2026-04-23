using TourCrm.Application.DTOs.Deals;
using TourCrm.Core.Specifications;

namespace TourCrm.Application.Interfaces;

public interface IDealService
{
    Task<List<DealDto>> GetAllAsync(int? statusId = null, int? officeId = null, int? managerId = null, CancellationToken ct = default);
    Task<DealDto?> GetAsync(int id, CancellationToken ct = default);
    Task<DealDto> CreateAsync(CreateDealDto dto, CancellationToken ct = default);
    Task UpdateAsync(int id, UpdateDealDto dto, CancellationToken ct = default);
    Task ChangeStatusAsync(int id, int statusId, CancellationToken ct = default);
    Task<DealDto> CreateFromLeadAsync(int leadId, int managerId, int touristId, CancellationToken ct = default);
    Task<List<DealHistoryDto>> GetHistoryAsync(int dealId, CancellationToken ct = default);
    Task<PagedResult<DealDto>> SearchAsync(DealSearchRequestDto dto, CancellationToken ct = default);
    Task ArchiveAsync(int id, CancellationToken ct = default);
    Task RestoreAsync(int id, CancellationToken ct = default);
}