using TourCrm.Application.DTOs.NumberType;

namespace TourCrm.Application.Interfaces.Dictionaries;

public interface INumberTypeService
{
    Task<List<NumberTypeDto>> GetAllAsync(CancellationToken ct = default);
    Task<NumberTypeDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<NumberTypeDto> CreateAsync(CreateNumberTypeDto dto, string userId, CancellationToken ct = default);
    Task UpdateAsync(int id, UpdateNumberTypeDto dto, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default); 
}