using TourCrm.Core.Entities;
using TourCrm.Core.Entities.Dictionaries;

namespace TourCrm.Core.Interfaces.Dictionaries;

public interface ILabelRepository
{
    Task<IReadOnlyList<Label>> GetAllAsync(CancellationToken ct);
    Task<Label?> GetByIdAsync(int id, CancellationToken ct);
    Task<Label> AddAsync(Label e, CancellationToken ct);
    Task UpdateAsync(Label e, CancellationToken ct);
    Task DeleteAsync(int id, CancellationToken ct);
}