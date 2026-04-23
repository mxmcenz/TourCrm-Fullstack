using TourCrm.Application.DTOs.Dictionaries.DealStatus;

namespace TourCrm.Application.Interfaces.Dictionaries;

public interface IDealStatusService
{
    Task<List<DealStatusDto>> GetAllAsync(CancellationToken ct = default);
    Task<DealStatusDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<DealStatusDto> CreateAsync(CreateDealStatusDto dto, string ownerUserId, CancellationToken ct = default);
    Task UpdateAsync(int id, UpdateDealStatusDto dto, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
    Task<int> SeedDefaultsForCompanyAsync(int companyId, CancellationToken ct = default);
}