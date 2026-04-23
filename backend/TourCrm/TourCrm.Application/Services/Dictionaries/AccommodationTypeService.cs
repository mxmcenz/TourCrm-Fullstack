using TourCrm.Application.DTOs.AccommodationType;
using TourCrm.Application.Interfaces.Dictionaries;
using TourCrm.Core.Entities.Dictionaries;
using TourCrm.Core.Interfaces;

namespace TourCrm.Application.Services.Dictionaries;

public class AccommodationTypeService(IUnitOfWork uow) : IAccommodationTypeService
{
    private static readonly string[] DefaultNames =
    [
        "Double", "Double + Ex.bed", "Single", "Twin", "Twin + Ex.Bed"
    ];

    public async Task<List<AccommodationTypeDto>> GetAllAsync(CancellationToken ct = default)
    {
        var all = await uow.AccommodationTypes.GetAllAsync(ct);

        return all
            .OrderBy(x => x.Name)
            .Select(x => new AccommodationTypeDto { Id = x.Id, Name = x.Name })
            .ToList();
    }

    public async Task<AccommodationTypeDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var e = await uow.AccommodationTypes.GetByIdAsync(id, ct);
        if (e == null) return null;

        return new AccommodationTypeDto { Id = e.Id, Name = e.Name };
    }

    public async Task<AccommodationTypeDto> CreateAsync(CreateAccommodationTypeDto dto, string userId, CancellationToken ct = default)
    {
        var company = await uow.Companies.GetByOwnerAsync(userId, ct)
                      ?? throw new InvalidOperationException("Компания не найдена для пользователя");
        var entity = new AccommodationType
        {
            Name = dto.Name.Trim(),
            CompanyId = company.Id
        };

        await uow.AccommodationTypes.AddAsync(entity, ct);
        await uow.SaveChangesAsync(ct);

        return new AccommodationTypeDto { Id = entity.Id, Name = entity.Name };
    }

    public async Task UpdateAsync(int id, UpdateAccommodationTypeDto dto, CancellationToken ct = default)
    {
        var entity = await uow.AccommodationTypes.GetByIdAsync(id, ct)
                     ?? throw new KeyNotFoundException("Accommodation type not found");

        entity.Name = dto.Name.Trim();
        uow.AccommodationTypes.Update(entity);
        await uow.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var entity = await uow.AccommodationTypes.GetByIdAsync(id, ct);
        if (entity == null) return;

        uow.AccommodationTypes.Delete(entity);
        await uow.SaveChangesAsync(ct);
    }
    
    public async Task<int> SeedDefaultsForCompanyAsync(int companyId, CancellationToken ct = default)
    {
        var existing = (await uow.AccommodationTypes.GetAllAsync(ct))
            .Where(x => x.CompanyId == companyId)
            .Select(x => x.Name.Trim().ToLower())
            .ToHashSet();

        var toAdd = DefaultNames
            .Select(n => n.Trim())
            .Where(n => !string.IsNullOrWhiteSpace(n) && !existing.Contains(n.ToLower()))
            .Select(n => new AccommodationType { Name = n, CompanyId = companyId })
            .ToList();

        if (toAdd.Count == 0) return 0;

        foreach (var st in toAdd)
            await uow.AccommodationTypes.AddAsync(st, ct);

        await uow.SaveChangesAsync(ct);
        return toAdd.Count;
    }
}