using TourCrm.Application.DTOs.Dictionaries.Labels;

namespace TourCrm.Application.Interfaces.Dictionaries;

public interface ILabelService
{
    Task<IReadOnlyList<LabelDto>> GetAllAsync(CancellationToken ct);
    Task<LabelDto> CreateAsync(CreateLabelDto dto, CancellationToken ct);
    Task UpdateAsync(int id, UpdateLabelDto dto, CancellationToken ct);
    Task DeleteAsync(int id, CancellationToken ct);
}