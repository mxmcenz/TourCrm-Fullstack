using TourCrm.Application.DTOs.ServiceType;

namespace TourCrm.Application.Interfaces.Dictionaries;

public interface IServiceTypeService
{
    Task<List<ServiceTypeDto>> GetAllAsync(CancellationToken ct = default);
    Task<ServiceTypeDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<ServiceTypeDto> CreateAsync(CreateServiceTypeDto dto, string userId, CancellationToken ct = default);
    Task UpdateAsync(int id, UpdateServiceTypeDto dto, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
    Task<int> SeedDefaultsForCompanyAsync(int companyId, CancellationToken ct = default);
}