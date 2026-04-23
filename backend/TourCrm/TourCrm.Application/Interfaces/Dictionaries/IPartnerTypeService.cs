using TourCrm.Application.DTOs.PartnerType;

namespace TourCrm.Application.Interfaces.Dictionaries;

public interface IPartnerTypeService
{
    Task<List<PartnerTypeDto>> GetAllAsync(CancellationToken ct = default);
    Task<PartnerTypeDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<PartnerTypeDto> CreateAsync(CreatePartnerTypeDto dto, string userId, CancellationToken ct = default);
    Task UpdateAsync(int id, UpdatePartnerTypeDto dto, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
    Task<int> SeedDefaultsForCompanyAsync(int companyId, CancellationToken ct = default);
}