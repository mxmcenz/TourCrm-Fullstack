using TourCrm.Application.DTOs.Dictionaries.DealStatus;
using TourCrm.Application.Interfaces.Dictionaries;
using TourCrm.Core.Entities.Dictionaries;
using TourCrm.Core.Interfaces;

namespace TourCrm.Application.Services.Dictionaries;

public class DealStatusService(IUnitOfWork uow) : IDealStatusService
{
    private static readonly (string Name, bool IsFinal)[] Defaults =
    [
        ("Новая", false),
        ("В работе", false),
        ("Завершена", true)
    ];

    public async Task<List<DealStatusDto>> GetAllAsync(CancellationToken ct = default)
    {
        var all = await uow.DealStatuses.GetAllAsync(ct);
        return all
            .OrderBy(x => x.Name)
            .Select(x => new DealStatusDto { Id = x.Id, Name = x.Name, IsFinal = x.IsFinal })
            .ToList();
    }

    public async Task<DealStatusDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var e = await uow.DealStatuses.GetByIdAsync(id, ct);
        return e == null ? null : new DealStatusDto { Id = e.Id, Name = e.Name, IsFinal = e.IsFinal };
    }

    public async Task<DealStatusDto> CreateAsync(CreateDealStatusDto dto, string ownerUserId, CancellationToken ct = default)
    {
        var company = await uow.Companies.GetByOwnerAsync(ownerUserId, ct)
                      ?? throw new InvalidOperationException("Компания не найдена для пользователя");

        var name = dto.Name?.Trim();
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required", nameof(dto.Name));

        var entity = new DealStatus { CompanyId = company.Id, Name = name!, IsFinal = dto.IsFinal };
        await uow.DealStatuses.AddAsync(entity, ct);
        await uow.SaveChangesAsync(ct);

        return new DealStatusDto { Id = entity.Id, Name = entity.Name, IsFinal = entity.IsFinal };
    }

    public async Task UpdateAsync(int id, UpdateDealStatusDto dto, CancellationToken ct = default)
    {
        var entity = await uow.DealStatuses.GetByIdAsync(id, ct)
                     ?? throw new KeyNotFoundException("Deal status not found");

        var name = dto.Name?.Trim();
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required", nameof(dto.Name));

        entity.Name = name!;
        entity.IsFinal = dto.IsFinal;

        uow.DealStatuses.Update(entity);
        await uow.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var entity = await uow.DealStatuses.GetByIdAsync(id, ct);
        if (entity == null) return;

        uow.DealStatuses.Delete(entity);
        await uow.SaveChangesAsync(ct);
    }

    public async Task<int> SeedDefaultsForCompanyAsync(int companyId, CancellationToken ct = default)
    {
        var existing = (await uow.DealStatuses.GetAllAsync(ct))
            .Where(x => x.CompanyId == companyId)
            .Select(x => x.Name.Trim().ToLower())
            .ToHashSet();

        var toAdd = Defaults
            .Where(d => !existing.Contains(d.Name.Trim().ToLower()))
            .Select(d => new DealStatus
            {
                CompanyId = companyId,
                Name = d.Name.Trim(),
                IsFinal = d.IsFinal
            })
            .ToList();

        if (toAdd.Count == 0) return 0;

        foreach (var s in toAdd)
            await uow.DealStatuses.AddAsync(s, ct);

        await uow.SaveChangesAsync(ct);
        return toAdd.Count;
    }
}