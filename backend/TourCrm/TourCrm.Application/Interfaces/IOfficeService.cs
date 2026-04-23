using TourCrm.Application.DTOs.Offices;

namespace TourCrm.Application.Interfaces;

public interface IOfficeService
{
    Task<IReadOnlyList<OfficeListItemDto>> GetByLegalAsync(
        string userId, int legalEntityId, string? q, CancellationToken ct = default);
    Task<OfficeDto?> GetAsync(int id, string userId, CancellationToken ct = default);
    Task<OfficeDto> CreateAsync(OfficeUpsertDto dto, string userId, CancellationToken ct = default);
    Task<OfficeDto> UpdateAsync(int id, OfficeUpsertDto dto, string userId, CancellationToken ct = default);
    Task SoftDeleteAsync(int id, string userId, CancellationToken ct = default);
}