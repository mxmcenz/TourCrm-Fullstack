using TourCrm.Application.DTOs.Partner;
using TourCrm.Application.Interfaces;
using TourCrm.Application.Interfaces.Dictionaries;
using TourCrm.Core.Entities;
using TourCrm.Core.Interfaces;

namespace TourCrm.Application.Services.Dictionaries;

public class PartnerService(IUnitOfWork uow , ICompanyService companyService) : IPartnerService
{
    public async Task<List<PartnerDto>> GetAllAsync(CancellationToken ct = default)
    {
        var all = await uow.Partners.GetAllAsync(ct);

        return all
            .OrderBy(x => x.Name)
            .Select(x => new PartnerDto { Id = x.Id, Name = x.Name })
            .ToList();
    }

    public async Task<PartnerDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var e = await uow.Partners.GetByIdAsync(id, ct);
        if (e == null) return null;

        return new PartnerDto { Id = e.Id, Name = e.Name };
    }

    public async Task<PartnerDto> CreateAsync(CreatePartnerDto dto, string userId, CancellationToken ct = default)
    {
        var company = await companyService.GetMineAsync(userId, ct)
                      ?? throw new InvalidOperationException("Компания не найдена для пользователя");
        var entity = new Partner
        {
            Name = dto.Name.Trim(),
            CompanyId = company.Id
        };

        await uow.Partners.AddAsync(entity, ct);
        await uow.SaveChangesAsync(ct);

        return new PartnerDto { Id = entity.Id, Name = entity.Name };
    }

    public async Task UpdateAsync(int id, UpdatePartnerDto dto, CancellationToken ct = default)
    {
        var entity = await uow.Partners.GetByIdAsync(id, ct)
                     ?? throw new KeyNotFoundException("Partner not found");

        entity.Name = dto.Name.Trim();
        uow.Partners.Update(entity);
        await uow.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var entity = await uow.Partners.GetByIdAsync(id, ct);
        if (entity == null) return;

        uow.Partners.Delete(entity);
        await uow.SaveChangesAsync(ct);
    }
}
