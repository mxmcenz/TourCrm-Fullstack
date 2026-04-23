using TourCrm.Core.Entities;
using TourCrm.Core.Entities.Roles;

namespace TourCrm.Core.Interfaces;

public interface IRolePermissionRepository : IGenericRepository<RolePermission>
{
    Task<IEnumerable<RolePermission>> GetByRoleIdAsync(int roleId);
    Task ReplaceForRoleAsync(int roleId, IEnumerable<RolePermission> items, CancellationToken ct = default);
    Task AddRangeAsync(IEnumerable<RolePermission> entities);
}