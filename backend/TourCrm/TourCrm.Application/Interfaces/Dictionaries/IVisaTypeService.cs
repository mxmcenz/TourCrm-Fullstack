using TourCrm.Application.DTOs.Dictionaries.VisaTypes;

namespace TourCrm.Application.Interfaces.Dictionaries;

public interface IVisaTypeService
{
    Task<IReadOnlyList<VisaTypeDto>> GetAllAsync(CancellationToken ct);
    Task<VisaTypeDto> CreateAsync(CreateVisaTypeDto dto, CancellationToken ct);
    Task UpdateAsync(int id, UpdateVisaTypeDto dto, CancellationToken ct);
    Task DeleteAsync(int id, CancellationToken ct);
}