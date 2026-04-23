using TourCrm.Application.DTOs.Citizenship;

namespace TourCrm.Application.Interfaces.Dictionaries;

public interface ICitizenshipService
{
    Task<List<CitizenshipDto>> GetAllAsync(CancellationToken ct = default);
    Task<CitizenshipDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<CitizenshipDto> CreateAsync(CreateCitizenshipDto dto, string userId, CancellationToken ct = default);
    Task UpdateAsync(int id, UpdateCitizenshipDto dto, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}