using TourCrm.Application.DTOs.AccommodationType;

namespace TourCrm.Application.Interfaces.Dictionaries;

public interface IAccommodationTypeService
{
    Task<List<AccommodationTypeDto>> GetAllAsync(CancellationToken ct = default);
    Task<AccommodationTypeDto?> GetByIdAsync(int id, CancellationToken ct = default);

    Task<AccommodationTypeDto> CreateAsync(CreateAccommodationTypeDto dto, string userId,
        CancellationToken ct = default);

    Task UpdateAsync(int id, UpdateAccommodationTypeDto dto, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
    Task<int> SeedDefaultsForCompanyAsync(int companyId, CancellationToken ct = default);
}