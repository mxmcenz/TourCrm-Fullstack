using TourCrm.Application.DTOs.Dictionaries.LeadRequestTypes;
using TourCrm.Application.Interfaces.Dictionaries;
using TourCrm.Core.Entities;
using TourCrm.Core.Entities.Dictionaries;
using TourCrm.Core.Interfaces.Dictionaries;

namespace TourCrm.Application.Services.Dictionaries;

public sealed class LeadRequestTypeService(ILeadRequestTypeRepository repo) : ILeadRequestTypeService
{
    public async Task<IReadOnlyList<LeadRequestTypeDto>> GetAllAsync(CancellationToken ct)
        => (await repo.GetAllAsync(ct)).Select(x => new LeadRequestTypeDto(x.Id, x.Name)).ToList();

    public async Task<LeadRequestTypeDto> CreateAsync(CreateLeadRequestTypeDto dto, CancellationToken ct)
    {
        var entity = await repo.AddAsync(new LeadRequestType { Name = dto.Name.Trim() }, ct);
        return new LeadRequestTypeDto(entity.Id, entity.Name);
    }

    public async Task UpdateAsync(int id, UpdateLeadRequestTypeDto dto, CancellationToken ct)
        => await repo.UpdateAsync(new LeadRequestType { Id = id, Name = dto.Name.Trim() }, ct);

    public Task DeleteAsync(int id, CancellationToken ct) => repo.DeleteAsync(id, ct);
}