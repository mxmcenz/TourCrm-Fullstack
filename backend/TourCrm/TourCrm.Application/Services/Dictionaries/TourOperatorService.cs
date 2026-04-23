using TourCrm.Application.DTOs.TourOperator;
using TourCrm.Application.Interfaces;
using TourCrm.Application.Interfaces.Dictionaries;
using TourCrm.Core.Entities;
using TourCrm.Core.Entities.Dictionaries;
using TourCrm.Core.Interfaces;

namespace TourCrm.Application.Services.Dictionaries;

public class TourOperatorService(IUnitOfWork uow , ICompanyService companyService) : ITourOperatorService
{
    public async Task<List<TourOperatorDto>> GetAllAsync(CancellationToken ct = default)
    {
        var all = await uow.TourOperators.GetAllAsync(ct);

        return all
            .OrderBy(x => x.Name)
            .Select(x => new TourOperatorDto { Id = x.Id, Name = x.Name })
            .ToList();
    }

    public async Task<TourOperatorDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var e = await uow.TourOperators.GetByIdAsync(id, ct);
        if (e == null) return null;

        return new TourOperatorDto { Id = e.Id, Name = e.Name };    }

    public async Task<TourOperatorDto> CreateAsync(CreateTourOperatorDto dto, string userId, CancellationToken ct = default)
    {
        var company = await companyService.GetMineAsync(userId, ct)
                      ?? throw new InvalidOperationException("Компания не найдена для пользователя");
        var entity = new TourOperator
        {
            Name = dto.Name.Trim(),
            CompanyId = company.Id
        };

        await uow.TourOperators.AddAsync(entity, ct);
        await uow.SaveChangesAsync(ct);

        return new TourOperatorDto { Id = entity.Id, Name = entity.Name };
    }

    public async Task UpdateAsync(int id, UpdateTourOperatorDto dto, CancellationToken ct = default)
    {
        var entity = await uow.TourOperators.GetByIdAsync(id, ct)
                     ?? throw new KeyNotFoundException("Number type not found");

        entity.Name = dto.Name.Trim();
        uow.TourOperators.Update(entity);
        await uow.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var entity = await uow.TourOperators.GetByIdAsync(id, ct);
        if (entity == null) return;

        uow.TourOperators.Delete(entity);
        await uow.SaveChangesAsync(ct);    
    }
}