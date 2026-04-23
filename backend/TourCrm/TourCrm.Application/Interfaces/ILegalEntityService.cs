using TourCrm.Application.DTOs.LegalEntity;

namespace TourCrm.Application.Interfaces;

public interface ILegalEntityService
{
    Task<IReadOnlyList<LegalEntityListItemDto>> GetMineAsync(
        string userId, string? q = null, CancellationToken ct = default);
    Task<LegalEntityDto?> GetAsync(int id, string userId, CancellationToken ct = default);
    Task<LegalEntityDto> CreateAsync(LegalEntityUpsertDto dto, string userId, CancellationToken ct = default);
    Task<LegalEntityDto> UpdateAsync(int id, LegalEntityUpsertDto dto, string userId, CancellationToken ct = default);
    Task SoftDeleteAsync(int id, string userId, CancellationToken ct = default);
}