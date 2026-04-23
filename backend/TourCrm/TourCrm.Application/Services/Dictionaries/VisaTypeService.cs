using TourCrm.Application.DTOs.Dictionaries.VisaTypes;
using TourCrm.Application.Interfaces.Dictionaries;
using TourCrm.Core.Entities;
using TourCrm.Core.Entities.Dictionaries;
using TourCrm.Core.Interfaces;
using TourCrm.Core.Interfaces.Dictionaries;

namespace TourCrm.Application.Services.Dictionaries;

public sealed class VisaTypeService(IVisaTypeRepository repo) : IVisaTypeService
{
    public async Task<IReadOnlyList<VisaTypeDto>> GetAllAsync(CancellationToken ct)
        => (await repo.GetAllAsync(ct)).Select(x => new VisaTypeDto(x.Id, x.Name)).ToList();

    public async Task<VisaTypeDto> CreateAsync(CreateVisaTypeDto dto, CancellationToken ct)
    {
        var entity = await repo.AddAsync(new VisaType { Name = dto.Name.Trim() }, ct);
        return new VisaTypeDto(entity.Id, entity.Name);
    }

    public Task UpdateAsync(int id, UpdateVisaTypeDto dto, CancellationToken ct)
        => repo.UpdateAsync(new VisaType { Id = id, Name = dto.Name.Trim() }, ct);

    public Task DeleteAsync(int id, CancellationToken ct) => repo.DeleteAsync(id, ct);
}