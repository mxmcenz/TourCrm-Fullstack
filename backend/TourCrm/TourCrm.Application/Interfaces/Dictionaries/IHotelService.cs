using TourCrm.Application.DTOs.Dictionaries.Hotels;

namespace TourCrm.Application.Interfaces.Dictionaries;

public interface IHotelService
{
    Task<IReadOnlyList<HotelDto>> GetAllAsync(CancellationToken ct);
    Task<HotelDto> CreateAsync(CreateHotelDto dto, CancellationToken ct);
    Task UpdateAsync(int id, UpdateHotelDto dto, CancellationToken ct);
    Task DeleteAsync(int id, CancellationToken ct);
}