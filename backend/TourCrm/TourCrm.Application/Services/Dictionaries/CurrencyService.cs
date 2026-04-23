using TourCrm.Application.DTOs.Dictionaries.Currencies;
using TourCrm.Application.Interfaces.Dictionaries;
using TourCrm.Core.Entities.Dictionaries;
using TourCrm.Core.Interfaces.Dictionaries;

namespace TourCrm.Application.Services.Dictionaries;

public sealed class CurrencyService(ICurrencyRepository repo) : ICurrencyService
{
    public async Task<IReadOnlyList<CurrencyDto>> GetAllAsync(CancellationToken ct)
        => (await repo.GetAllAsync(ct)).Select(x => new CurrencyDto(x.Id, x.Name)).ToList();

    public async Task<CurrencyDto> CreateAsync(CreateCurrencyDto dto, CancellationToken ct)
    {
        var entity = await repo.AddAsync(new Currency { Name = dto.Name.Trim() }, ct);
        return new CurrencyDto(entity.Id, entity.Name);
    }

    public async Task UpdateAsync(int id, UpdateCurrencyDto dto, CancellationToken ct)
        => await repo.UpdateAsync(new Currency { Id = id, Name = dto.Name.Trim() }, ct);

    public Task DeleteAsync(int id, CancellationToken ct) => repo.DeleteAsync(id, ct);
}