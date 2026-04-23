using TourCrm.Core.Entities.Dictionaries;

namespace TourCrm.Core.Interfaces.Dictionaries;

public interface IHotelRepository
{
    Task<IReadOnlyList<Hotel>> GetAllAsync(CancellationToken ct);
    Task<Hotel?> GetByIdAsync(int id, CancellationToken ct);
    Task<Hotel> AddAsync(Hotel e, CancellationToken ct);
    Task UpdateAsync(Hotel e, CancellationToken ct);
    Task DeleteAsync(int id, CancellationToken ct);
}