using TourCrm.Application.DTOs.NumberType;
using TourCrm.Application.Interfaces;
using TourCrm.Application.Interfaces.Dictionaries;
using TourCrm.Core.Entities.Dictionaries;
using TourCrm.Core.Interfaces;

namespace TourCrm.Application.Services.Dictionaries;

public class NumberTypeService (IUnitOfWork uow, ICompanyService companyService) : INumberTypeService
{
    public async Task<List<NumberTypeDto>> GetAllAsync(CancellationToken ct = default)
    {
        var all = await uow.NumberTypes.GetAllAsync(ct);

        return all
            .OrderBy(x => x.Name)
            .Select(x => new NumberTypeDto { Id = x.Id, Name = x.Name })
            .ToList();
    }

    public async Task<NumberTypeDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var e = await uow.NumberTypes.GetByIdAsync(id, ct);
        if (e == null) return null;

        return new NumberTypeDto { Id = e.Id, Name = e.Name };    }

    public async Task<NumberTypeDto> CreateAsync(CreateNumberTypeDto dto, string userId, CancellationToken ct = default)
    {
        var company = await companyService.GetMineAsync(userId, ct)
                      ?? throw new InvalidOperationException("Компания не найдена для пользователя");
        var entity = new NumberType
        {
            Name = dto.Name.Trim(),
            CompanyId = company.Id
        };

        await uow.NumberTypes.AddAsync(entity, ct);
        await uow.SaveChangesAsync(ct);

        return new NumberTypeDto { Id = entity.Id, Name = entity.Name };
    }

    public async Task UpdateAsync(int id, UpdateNumberTypeDto dto, CancellationToken ct = default)
    {
        var entity = await uow.NumberTypes.GetByIdAsync(id, ct)
                     ?? throw new KeyNotFoundException("Number type not found");

        entity.Name = dto.Name.Trim();
        uow.NumberTypes.Update(entity);
        await uow.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var entity = await uow.NumberTypes.GetByIdAsync(id, ct);
        if (entity == null) return;

        uow.NumberTypes.Delete(entity);
        await uow.SaveChangesAsync(ct);    }
}