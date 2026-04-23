using TourCrm.Application.DTOs.Dictionaries.Countries;

namespace TourCrm.Application.Interfaces.Dictionaries;

public interface ICountryService
{
    Task<IReadOnlyList<CountryDto>> GetAllAsync(CancellationToken ct);
    Task<CountryDto> CreateAsync(CreateCountryDto dto, CancellationToken ct);
    Task UpdateAsync(int id, UpdateCountryDto dto, CancellationToken ct);
    Task DeleteAsync(int id, CancellationToken ct);
}