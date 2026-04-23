using TourCrm.Application.DTOs.PartnerMark;
using TourCrm.Application.Interfaces;
using TourCrm.Application.Interfaces.Dictionaries;
using TourCrm.Core.Entities;
using TourCrm.Core.Entities.Dictionaries;
using TourCrm.Core.Interfaces;

namespace TourCrm.Application.Services.Dictionaries;

public class PartnerMarkService(IUnitOfWork uow , ICompanyService companyService) : IPartnerMarkService
{
    public async Task<List<PartnerMarkDto>> GetAllAsync(CancellationToken ct = default)
    {
        var all = await uow.PartnerMarks.GetAllAsync(ct);

        return all
            .OrderBy(x => x.Name)
            .Select(x => new PartnerMarkDto { Id = x.Id, Name = x.Name })
            .ToList();
    }

    public async Task<PartnerMarkDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var e = await uow.PartnerMarks.GetByIdAsync(id, ct);
        if (e == null) return null;

        return new PartnerMarkDto { Id = e.Id, Name = e.Name };
    }

    public async Task<PartnerMarkDto> CreateAsync(CreatePartnerMarkDto dto, string userId, CancellationToken ct = default)
    {
        var company = await companyService.GetMineAsync(userId, ct)
                      ?? throw new InvalidOperationException("Компания не найдена для пользователя");
        var entity = new PartnerMark
        {
            Name = dto.Name.Trim(),
            CompanyId = company.Id
        };

        await uow.PartnerMarks.AddAsync(entity, ct);
        await uow.SaveChangesAsync(ct);

        return new PartnerMarkDto { Id = entity.Id, Name = entity.Name };
    }

    public async Task UpdateAsync(int id, UpdatePartnerMarkDto dto, CancellationToken ct = default)
    {
        var entity = await uow.PartnerMarks.GetByIdAsync(id, ct)
                     ?? throw new KeyNotFoundException("Partner mark not found");

        entity.Name = dto.Name.Trim();
        uow.PartnerMarks.Update(entity);
        await uow.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var entity = await uow.PartnerMarks.GetByIdAsync(id, ct);
        if (entity == null) return;

        uow.PartnerMarks.Delete(entity);
        await uow.SaveChangesAsync(ct);
    }
}
