using TourCrm.Application.DTOs.PartnerType;
using TourCrm.Application.Interfaces.Dictionaries;
using TourCrm.Core.Entities;
using TourCrm.Core.Entities.Dictionaries;
using TourCrm.Core.Interfaces;

namespace TourCrm.Application.Services.Dictionaries;

public class PartnerTypeService(IUnitOfWork uow) : IPartnerTypeService
{
    private static readonly string[] DefaultNames =
    [
        "Авиакомпании", "Страховые компании", "Транспортные компании", "Турагенства", "Туроператоры"
    ];

    public async Task<List<PartnerTypeDto>> GetAllAsync(CancellationToken ct = default)
    {
        var all = await uow.PartnerTypes.GetAllAsync(ct);

        return all
            .OrderBy(x => x.Name)
            .Select(x => new PartnerTypeDto { Id = x.Id, Name = x.Name })
            .ToList();
    }

    public async Task<PartnerTypeDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var e = await uow.PartnerTypes.GetByIdAsync(id, ct);
        if (e == null) return null;

        return new PartnerTypeDto { Id = e.Id, Name = e.Name };
    }

    public async Task<PartnerTypeDto> CreateAsync(CreatePartnerTypeDto dto, string userId, CancellationToken ct = default)
    {
        var company = await uow.Companies.GetByOwnerAsync(userId, ct)
                      ?? throw new InvalidOperationException("Компания не найдена для пользователя");
        var entity = new PartnerType
        {
            Name = dto.Name.Trim(),
            CompanyId = company.Id
        };

        await uow.PartnerTypes.AddAsync(entity, ct);
        await uow.SaveChangesAsync(ct);

        return new PartnerTypeDto { Id = entity.Id, Name = entity.Name };
    }

    public async Task UpdateAsync(int id, UpdatePartnerTypeDto dto, CancellationToken ct = default)
    {
        var entity = await uow.PartnerTypes.GetByIdAsync(id, ct)
                     ?? throw new KeyNotFoundException("Partner type not found");

        entity.Name = dto.Name.Trim();
        uow.PartnerTypes.Update(entity);
        await uow.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var entity = await uow.PartnerTypes.GetByIdAsync(id, ct);
        if (entity == null) return;

        uow.PartnerTypes.Delete(entity);
        await uow.SaveChangesAsync(ct);
    }

    public async Task<int> SeedDefaultsForCompanyAsync(int companyId, CancellationToken ct = default)
    {
        var existing = (await uow.PartnerTypes.GetAllAsync(ct))
            .Where(x => x.CompanyId == companyId)
            .Select(x => x.Name.Trim().ToLower())
            .ToHashSet();

        var toAdd = DefaultNames
            .Select(n => n.Trim())
            .Where(n => !string.IsNullOrWhiteSpace(n) && !existing.Contains(n.ToLower()))
            .Select(n => new PartnerType { Name = n, CompanyId = companyId })
            .ToList();

        if (toAdd.Count == 0) return 0;

        foreach (var st in toAdd)
            await uow.PartnerTypes.AddAsync(st, ct);

        await uow.SaveChangesAsync(ct);
        return toAdd.Count;
    }
}