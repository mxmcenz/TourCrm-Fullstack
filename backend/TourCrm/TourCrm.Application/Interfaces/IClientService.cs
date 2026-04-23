using TourCrm.Application.DTOs.Clients;
using TourCrm.Core.Entities.Client;

namespace TourCrm.Application.Interfaces;

public interface IClientService
{
    Task<(IReadOnlyList<ClientListItemDto> items, int total)> SearchAsync(
        int companyId, string? query, int page, int pageSize, bool includeDeleted, CancellationToken ct);
    Task<(IReadOnlyList<ClientListItemDto> items, int total)> SearchDeletedAsync(int companyId, string? query, int page,
        int pageSize, CancellationToken ct);
    Task<ClientDetailsDto?> GetAsync(int id, int companyId, bool includeDeleted, CancellationToken ct);
    Task<ClientDetailsDto> CreateAsync(int companyId, int? userId, CreateClientDto dto, CancellationToken ct);
    Task UpdateAsync(int id, int companyId, int? userId, UpdateClientDto dto, CancellationToken ct);
    Task SoftDeleteAsync(int id, int companyId, int? userId, CancellationToken ct);
    Task RestoreAsync(int id, int companyId, int? userId, CancellationToken ct);
}