using TourCrm.Application.DTOs.MealType;

namespace TourCrm.Application.Interfaces.Dictionaries;

public interface IMealTypeService
{
    Task<List<MealTypeDto>> GetAllAsync(CancellationToken ct = default);
    Task<MealTypeDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<MealTypeDto> CreateAsync(CreateMealTypeDto dto, string userId, CancellationToken ct = default);
    Task UpdateAsync(int id, UpdateMealTypeDto dto, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
    Task<int> SeedDefaultsForCompanyAsync(int companyId, CancellationToken ct = default);
}