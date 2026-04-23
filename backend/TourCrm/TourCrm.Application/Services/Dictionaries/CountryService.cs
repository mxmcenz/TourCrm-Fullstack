using TourCrm.Application.DTOs.Dictionaries.Countries;
using TourCrm.Application.Interfaces.Dictionaries;
using TourCrm.Core.Entities;
using TourCrm.Core.Entities.Dictionaries;
using TourCrm.Core.Interfaces.Dictionaries;

namespace TourCrm.Application.Services.Dictionaries;

public sealed class CountryService(ICountryRepository repo) : ICountryService
{
    public async Task<IReadOnlyList<CountryDto>> GetAllAsync(CancellationToken ct)
        => (await repo.GetAllAsync(ct)).Select(x => new CountryDto(x.Id, x.Name)).ToList();

    public async Task<CountryDto> CreateAsync(CreateCountryDto dto, CancellationToken ct)
    {
        var entity = await repo.AddAsync(new Country { Name = dto.Name.Trim() }, ct);
        return new CountryDto(entity.Id, entity.Name);
    }

    public async Task UpdateAsync(int id, UpdateCountryDto dto, CancellationToken ct)
        => await repo.UpdateAsync(new Country { Id = id, Name = dto.Name.Trim() }, ct);

    public Task DeleteAsync(int id, CancellationToken ct) => repo.DeleteAsync(id, ct);
}