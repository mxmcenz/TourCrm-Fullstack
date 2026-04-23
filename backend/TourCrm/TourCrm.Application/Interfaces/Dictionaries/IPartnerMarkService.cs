using TourCrm.Application.DTOs.PartnerMark;

namespace TourCrm.Application.Interfaces.Dictionaries;

public interface IPartnerMarkService
{
    Task<List<PartnerMarkDto>> GetAllAsync(CancellationToken ct = default);
    Task<PartnerMarkDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<PartnerMarkDto> CreateAsync(CreatePartnerMarkDto dto, string userId, CancellationToken ct = default);
    Task UpdateAsync(int id, UpdatePartnerMarkDto dto, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}