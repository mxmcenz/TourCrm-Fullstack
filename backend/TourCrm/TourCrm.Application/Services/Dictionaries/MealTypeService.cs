using TourCrm.Application.DTOs.MealType;
using TourCrm.Application.Interfaces.Dictionaries;
using TourCrm.Core.Entities.Dictionaries;
using TourCrm.Core.Interfaces;

namespace TourCrm.Application.Services.Dictionaries;

public class MealTypeService(IUnitOfWork uow ) : IMealTypeService
{
    private static readonly string[] DefaultNames =
    [
        "Без питания", "Все включено", "Завтрак", "Завтрак и ужин", "Завтрак и ужин + напитки",
        "Завтрак, обед и ужин", "Завтрак, обед и ужин + напитки", "Ультра все включено"
    ];

    public async Task<List<MealTypeDto>> GetAllAsync(CancellationToken ct = default)
    {
        var all = await uow.MealTypes.GetAllAsync(ct);

        return all
            .OrderBy(x => x.Name)
            .Select(x => new MealTypeDto { Id = x.Id, Name = x.Name })
            .ToList();
    }

    public async Task<MealTypeDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var e = await uow.MealTypes.GetByIdAsync(id, ct);
        if (e == null) return null;

        return new MealTypeDto { Id = e.Id, Name = e.Name };
    }

    public async Task<MealTypeDto> CreateAsync(CreateMealTypeDto dto, string userId, CancellationToken ct = default)
    {
        var company = await uow.Companies.GetByOwnerAsync(userId, ct)
                      ?? throw new InvalidOperationException("Компания не найдена для пользователя");
        var entity = new MealType
        {
            Name = dto.Name.Trim(),
            CompanyId = company.Id
        };

        await uow.MealTypes.AddAsync(entity, ct);
        await uow.SaveChangesAsync(ct);

        return new MealTypeDto { Id = entity.Id, Name = entity.Name };
    }

    public async Task UpdateAsync(int id, UpdateMealTypeDto dto, CancellationToken ct = default)
    {
        var entity = await uow.MealTypes.GetByIdAsync(id, ct)
                     ?? throw new KeyNotFoundException("Meal type not found");

        entity.Name = dto.Name.Trim();
        uow.MealTypes.Update(entity);
        await uow.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var entity = await uow.MealTypes.GetByIdAsync(id, ct);
        if (entity == null) return;

        uow.MealTypes.Delete(entity);
        await uow.SaveChangesAsync(ct);
    }
    
    public async Task<int> SeedDefaultsForCompanyAsync(int companyId, CancellationToken ct = default)
    {
        var existing = (await uow.MealTypes.GetAllAsync(ct))
            .Where(x => x.CompanyId == companyId)
            .Select(x => x.Name.Trim().ToLower())
            .ToHashSet();

        var toAdd = DefaultNames
            .Select(n => n.Trim())
            .Where(n => !string.IsNullOrWhiteSpace(n) && !existing.Contains(n.ToLower()))
            .Select(n => new MealType { Name = n, CompanyId = companyId })
            .ToList();

        if (toAdd.Count == 0) return 0;

        foreach (var st in toAdd)
            await uow.MealTypes.AddAsync(st, ct);

        await uow.SaveChangesAsync(ct);
        return toAdd.Count;
    }
}