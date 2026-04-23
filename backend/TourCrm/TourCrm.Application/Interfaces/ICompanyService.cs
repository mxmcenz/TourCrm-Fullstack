using TourCrm.Core.Entities;

namespace TourCrm.Application.Interfaces;

public interface ICompanyService
{
    Task<Company?> GetMineAsync(string userId, CancellationToken ct = default);
    Task<Company> CreateAsync(string userId, string name, CancellationToken ct = default);
    Task SetMainLegalAsync(string userId, int legalEntityId, CancellationToken ct = default);
    Task<Company> RenameAsync(string userId, string name, CancellationToken ct = default);
    Task<Company> CreateIfMissingAsync(string userId, string name, CancellationToken ct = default);
}