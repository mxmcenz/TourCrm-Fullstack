using TourCrm.Application.DTOs.City;

namespace TourCrm.Application.Interfaces.Dictionaries;

public interface ICityService
{
    Task<List<CityDto>> GetAllAsync(CancellationToken ct = default);
    Task<CityDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<CityDto> CreateAsync(CreateCityDto dto, string userId, CancellationToken ct = default);
    Task UpdateAsync(int id, UpdateCityDto dto, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}