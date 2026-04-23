using TourCrm.Application.DTOs.Dictionaries.Currencies;

namespace TourCrm.Application.Interfaces.Dictionaries;

public interface ICurrencyService
{
    Task<IReadOnlyList<CurrencyDto>> GetAllAsync(CancellationToken ct);
    Task<CurrencyDto> CreateAsync(CreateCurrencyDto dto, CancellationToken ct);
    Task UpdateAsync(int id, UpdateCurrencyDto dto, CancellationToken ct);
    Task DeleteAsync(int id, CancellationToken ct);
}