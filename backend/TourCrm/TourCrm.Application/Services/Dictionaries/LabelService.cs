using TourCrm.Application.DTOs.Dictionaries.Labels;
using TourCrm.Application.Interfaces.Dictionaries;
using TourCrm.Core.Entities;
using TourCrm.Core.Entities.Dictionaries;
using TourCrm.Core.Interfaces.Dictionaries;

namespace TourCrm.Application.Services.Dictionaries;

public sealed class LabelService(ILabelRepository repo) : ILabelService
{
    public async Task<IReadOnlyList<LabelDto>> GetAllAsync(CancellationToken ct)
        => (await repo.GetAllAsync(ct)).Select(x => new LabelDto(x.Id, x.Name)).ToList();

    public async Task<LabelDto> CreateAsync(CreateLabelDto dto, CancellationToken ct)
    {
        var entity = await repo.AddAsync(new Label { Name = dto.Name.Trim() }, ct);
        return new LabelDto(entity.Id, entity.Name);
    }

    public async Task UpdateAsync(int id, UpdateLabelDto dto, CancellationToken ct)
        => await repo.UpdateAsync(new Label { Id = id, Name = dto.Name.Trim() }, ct);

    public Task DeleteAsync(int id, CancellationToken ct) => repo.DeleteAsync(id, ct);
}