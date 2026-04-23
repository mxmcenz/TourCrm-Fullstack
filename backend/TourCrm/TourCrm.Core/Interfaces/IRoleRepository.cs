using TourCrm.Core.Entities;
using TourCrm.Core.Entities.Roles;

namespace TourCrm.Core.Interfaces;

public interface IRoleRepository : IGenericRepository<Role>
{
    Task<(List<Role> Items, int Total)> GetPagedAsync(
        int page,
        int pageSize,
        string? search,
        string? sortBy,
        bool desc,
        string superAdminName,
        int? companyId);
}