using TourCrm.Application.DTOs.City;
using TourCrm.Application.Interfaces;
using TourCrm.Application.Interfaces.Dictionaries;
using TourCrm.Core.Entities.Dictionaries;
using TourCrm.Core.Interfaces;

namespace TourCrm.Application.Services.Dictionaries;

public class CityService(IUnitOfWork uow , ICompanyService companyService) : ICityService
{ 
    public async Task<List<CityDto>> GetAllAsync(CancellationToken ct = default)
    {
        var all = await uow.Cities.GetAllAsync(ct);

        return all
            .OrderBy(x => x.Name)
            .Select(x => new CityDto
            {
                Id = x.Id,
                Name = x.Name,
                CountryId = x.CountryId,
                CountryName = x.Country != null ? x.Country.Name : null
            })
            .ToList();
    }

    public async Task<CityDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var c = await uow.Cities.GetByIdAsync(id, ct);
        if (c == null) return null;

        var country = await uow.Countries.GetByIdAsync(c.CountryId, ct);

        return new CityDto
        {
            Id = c.Id,
            Name = c.Name,
            CountryId = c.CountryId,
            CountryName = c.Country.Name
        };
    }

    public async Task<CityDto> CreateAsync(CreateCityDto dto, string userId, CancellationToken ct = default)
    {
        var company = await companyService.GetMineAsync(userId, ct)
                      ?? throw new InvalidOperationException("Компания не найдена для пользователя");
        _ = await uow.Countries.GetByIdAsync(dto.CountryId, ct)
            ?? throw new KeyNotFoundException("Country not found");

        var entity = new City
        {
            Name = dto.Name.Trim(),
            CountryId = dto.CountryId,
            CompanyId = company.Id
        };

        await uow.Cities.AddAsync(entity, ct);
        await uow.SaveChangesAsync(ct);

        return new CityDto
        {
            Id = entity.Id,
            Name = entity.Name,
            CountryId = entity.CountryId
        };
    }

    public async Task UpdateAsync(int id, UpdateCityDto dto, CancellationToken ct = default)
    {
        var entity = await uow.Cities.GetByIdAsync(id, ct) ?? throw new KeyNotFoundException("City not found");

        _ = await uow.Countries.GetByIdAsync(dto.CountryId, ct)
            ?? throw new KeyNotFoundException("Country not found");

        entity.Name = dto.Name.Trim();
        entity.CountryId = dto.CountryId;

        uow.Cities.Update(entity);
        await uow.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var entity = await uow.Cities.GetByIdAsync(id, ct);
        if (entity == null) return;

        uow.Cities.Delete(entity);
        await uow.SaveChangesAsync(ct);
    }
}
