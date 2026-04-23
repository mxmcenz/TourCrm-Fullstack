using TourCrm.Application.DTOs.Dictionaries.LeadSources;
using TourCrm.Application.Interfaces.Dictionaries;
using TourCrm.Core.Entities;
using TourCrm.Core.Entities.Dictionaries;
using TourCrm.Core.Interfaces.Dictionaries;

namespace TourCrm.Application.Services.Dictionaries;

public sealed class LeadSourceService(ILeadSourceRepository repo) : ILeadSourceService
{
    public async Task<IReadOnlyList<LeadSourceDto>> GetAllAsync(CancellationToken ct)
        => (await repo.GetAllAsync(ct)).Select(x => new LeadSourceDto(x.Id, x.Name)).ToList();

    public async Task<LeadSourceDto> CreateAsync(CreateLeadSourceDto dto, CancellationToken ct)
    {
        var entity = await repo.AddAsync(new LeadSource { Name = dto.Name.Trim() }, ct);
        return new LeadSourceDto(entity.Id, entity.Name);
    }

    public async Task UpdateAsync(int id, UpdateLeadSourceDto dto, CancellationToken ct)
        => await repo.UpdateAsync(new LeadSource { Id = id, Name = dto.Name.Trim() }, ct);

    public Task DeleteAsync(int id, CancellationToken ct) => repo.DeleteAsync(id, ct);
}