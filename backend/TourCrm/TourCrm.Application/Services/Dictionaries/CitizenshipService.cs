using TourCrm.Application.DTOs.Citizenship;
using TourCrm.Application.Interfaces;
using TourCrm.Application.Interfaces.Dictionaries;
using TourCrm.Core.Entities;
using TourCrm.Core.Entities.Dictionaries;
using TourCrm.Core.Interfaces;

namespace TourCrm.Application.Services.Dictionaries;

public class CitizenshipService(IUnitOfWork uow , ICompanyService companyService) : ICitizenshipService
{
    public async Task<List<CitizenshipDto>> GetAllAsync(CancellationToken ct = default)
    {
        var all = await uow.Citizenships.GetAllAsync(ct);

        return all
            .OrderBy(x => x.Name)
            .Select(x => new CitizenshipDto { Id = x.Id, Name = x.Name })
            .ToList();
    }

    public async Task<CitizenshipDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var e = await uow.Citizenships.GetByIdAsync(id, ct);
        if (e == null) return null;

        return new CitizenshipDto { Id = e.Id, Name = e.Name };
    }

    public async Task<CitizenshipDto> CreateAsync(CreateCitizenshipDto dto, string userId, CancellationToken ct = default)
    {
        var company = await companyService.GetMineAsync(userId, ct)
                      ?? throw new InvalidOperationException("Компания не найдена для пользователя");
        var entity = new Citizenship
        {
            Name = dto.Name.Trim(),
            CompanyId = company.Id
        };

        await uow.Citizenships.AddAsync(entity, ct);
        await uow.SaveChangesAsync(ct);

        return new CitizenshipDto { Id = entity.Id, Name = entity.Name };
    }

    public async Task UpdateAsync(int id, UpdateCitizenshipDto dto, CancellationToken ct = default)
    {
        var entity = await uow.Citizenships.GetByIdAsync(id, ct)
                     ?? throw new KeyNotFoundException("Service type not found");

        entity.Name = dto.Name.Trim();
        uow.Citizenships.Update(entity);
        await uow.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var entity = await uow.Citizenships.GetByIdAsync(id, ct);
        if (entity == null) return;

        uow.Citizenships.Delete(entity);
        await uow.SaveChangesAsync(ct);
    }
}