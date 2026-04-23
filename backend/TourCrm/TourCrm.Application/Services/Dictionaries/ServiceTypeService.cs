using TourCrm.Application.DTOs.ServiceType;
using TourCrm.Application.Interfaces.Dictionaries;
using TourCrm.Core.Entities.Dictionaries;
using TourCrm.Core.Interfaces;

namespace TourCrm.Application.Services.Dictionaries;

public class ServiceTypeService(IUnitOfWork uow ) : IServiceTypeService
{
    private static readonly string[] DefaultNames =
    [
        "SPA","VIP-зал","Авиабилет","Автобус","Виза","Доп. услуга","ЖД билет","Загранпаспорт",
        "Комиссия банка","Консультация","Круиз","Отель","Питание","Свадебная услуга","Страховка",
        "Трансфер","Экскурсия"
    ];

    public async Task<List<ServiceTypeDto>> GetAllAsync(CancellationToken ct = default)
    {
        var all = await uow.ServiceTypes.GetAllAsync(ct);

        return all
            .OrderBy(x => x.Name)
            .Select(x => new ServiceTypeDto { Id = x.Id, Name = x.Name })
            .ToList();
    }

    public async Task<ServiceTypeDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var e = await uow.ServiceTypes.GetByIdAsync(id, ct);
        if (e == null) return null;

        return new ServiceTypeDto { Id = e.Id, Name = e.Name };
    }

    public async Task<ServiceTypeDto> CreateAsync(CreateServiceTypeDto dto, string userId, CancellationToken ct = default)
    {
        var company = await uow.Companies.GetByOwnerAsync(userId, ct)
                      ?? throw new InvalidOperationException("Компания не найдена для пользователя");
        var entity = new ServiceType
        {
            Name = dto.Name.Trim(),
            CompanyId = company.Id
        };

        await uow.ServiceTypes.AddAsync(entity, ct);
        await uow.SaveChangesAsync(ct);

        return new ServiceTypeDto { Id = entity.Id, Name = entity.Name };
    }

    public async Task UpdateAsync(int id, UpdateServiceTypeDto dto, CancellationToken ct = default)
    {
        var entity = await uow.ServiceTypes.GetByIdAsync(id, ct)
                     ?? throw new KeyNotFoundException("Service type not found");

        entity.Name = dto.Name.Trim();
        uow.ServiceTypes.Update(entity);
        await uow.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var entity = await uow.ServiceTypes.GetByIdAsync(id, ct);
        if (entity == null) return;

        uow.ServiceTypes.Delete(entity);
        await uow.SaveChangesAsync(ct);
    }

    public async Task<int> SeedDefaultsForCompanyAsync(int companyId, CancellationToken ct = default)
    {
        var existing = (await uow.ServiceTypes.GetAllAsync(ct))
            .Where(x => x.CompanyId == companyId)
            .Select(x => x.Name.Trim().ToLower())
            .ToHashSet();

        var toAdd = DefaultNames
            .Select(n => n.Trim())
            .Where(n => !string.IsNullOrWhiteSpace(n) && !existing.Contains(n.ToLower()))
            .Select(n => new ServiceType { Name = n, CompanyId = companyId })
            .ToList();

        if (toAdd.Count == 0) return 0;

        foreach (var st in toAdd)
            await uow.ServiceTypes.AddAsync(st, ct);

        await uow.SaveChangesAsync(ct);
        return toAdd.Count;
    }
}
