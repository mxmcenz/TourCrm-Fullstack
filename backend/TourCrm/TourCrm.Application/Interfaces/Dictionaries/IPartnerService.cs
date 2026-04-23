using TourCrm.Application.DTOs.Partner;

namespace TourCrm.Application.Interfaces.Dictionaries;

public interface IPartnerService
{
    Task<List<PartnerDto>> GetAllAsync(CancellationToken ct = default);
    Task<PartnerDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<PartnerDto> CreateAsync(CreatePartnerDto dto, string userId, CancellationToken ct = default);
    Task UpdateAsync(int id, UpdatePartnerDto dto, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}