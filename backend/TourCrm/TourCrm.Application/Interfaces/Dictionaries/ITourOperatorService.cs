using TourCrm.Application.DTOs.TourOperator;

namespace TourCrm.Application.Interfaces.Dictionaries;

public interface ITourOperatorService
{
    Task<List<TourOperatorDto>> GetAllAsync(CancellationToken ct = default);
    Task<TourOperatorDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<TourOperatorDto> CreateAsync(CreateTourOperatorDto dto, string userId, CancellationToken ct = default);
    Task UpdateAsync(int id, UpdateTourOperatorDto dto, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}