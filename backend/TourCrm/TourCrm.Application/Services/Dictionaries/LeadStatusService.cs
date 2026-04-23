using TourCrm.Application.DTOs.Dictionaries.LeadStatuses;
using TourCrm.Application.Interfaces.Dictionaries;
using TourCrm.Core.Entities;
using TourCrm.Core.Entities.Dictionaries;
using TourCrm.Core.Interfaces.Dictionaries;

namespace TourCrm.Application.Services.Dictionaries;

public sealed class LeadStatusService(ILeadStatusRepository repo) : ILeadStatusService
{
    public async Task<IReadOnlyList<LeadStatusDto>> GetAllAsync(CancellationToken ct)
        => (await repo.GetAllAsync(ct)).Select(x => new LeadStatusDto(x.Id, x.Name)).ToList();

    public async Task<LeadStatusDto> CreateAsync(CreateLeadStatusDto dto, CancellationToken ct)
    {
        var entity = await repo.AddAsync(new LeadStatus { Name = dto.Name.Trim() }, ct);
        return new LeadStatusDto(entity.Id, entity.Name);
    }

    public async Task UpdateAsync(int id, UpdateLeadStatusDto dto, CancellationToken ct)
        => await repo.UpdateAsync(new LeadStatus { Id = id, Name = dto.Name.Trim() }, ct);

    public Task DeleteAsync(int id, CancellationToken ct) => repo.DeleteAsync(id, ct);
}