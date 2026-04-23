using TourCrm.Core.Entities.Client;

namespace TourCrm.Core.Interfaces;

public interface IClientRepository : IGenericRepository<Client>
{
    Task<Client?> GetByIdAsync(int companyId, int id, CancellationToken ct = default);
    Task<Client?> GetWithDetailsAsync(int companyId, int id, bool includeDeleted, CancellationToken ct);

    Task<bool> EmailExistsAsync(int companyId, string email, int? excludeClientId = null,
        CancellationToken ct = default);

    Task<bool> PhoneExistsAsync(int companyId, string phoneE164, int? excludeClientId = null,
        CancellationToken ct = default);

    Task<IReadOnlyList<Client>> SearchAsync(int companyId, string? query, int page, int pageSize,
        CancellationToken ct = default);

    Task<int> CountAsync(int companyId, string? query, CancellationToken ct = default);
    Task<Client?> GetForUpdateAsync(int companyId, int id, CancellationToken ct = default);
    Task<Client?> GetForUpdateAsync(int companyId, int id, bool includeDeleted, CancellationToken ct);

    Task<(IReadOnlyList<Client> items, int total)> SearchWithDetailsAsync(
        int companyId, string? query, int page, int pageSize, bool includeDeleted, CancellationToken ct);

    Task<(IReadOnlyList<Client> items, int total)> SearchDeletedAsync(
        int companyId, string? q, int page, int pageSize, CancellationToken ct);
}