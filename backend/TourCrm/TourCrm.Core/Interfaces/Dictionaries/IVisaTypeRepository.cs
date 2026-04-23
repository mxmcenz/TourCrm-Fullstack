using TourCrm.Core.Entities.Dictionaries;

namespace TourCrm.Core.Interfaces.Dictionaries;

public interface IVisaTypeRepository
{
    Task<IReadOnlyList<VisaType>> GetAllAsync(CancellationToken ct);
    Task<VisaType?> GetByIdAsync(int id, CancellationToken ct);
    Task<VisaType> AddAsync(VisaType e, CancellationToken ct);
    Task UpdateAsync(VisaType e, CancellationToken ct);
    Task DeleteAsync(int id, CancellationToken ct);
}