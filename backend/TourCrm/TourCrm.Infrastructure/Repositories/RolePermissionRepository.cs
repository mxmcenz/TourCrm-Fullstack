using Microsoft.EntityFrameworkCore;
using TourCrm.Core.Entities;
using TourCrm.Core.Entities.Roles;
using TourCrm.Core.Interfaces;
using TourCrm.Infrastructure.Data;

namespace TourCrm.Infrastructure.Repositories;

public class RolePermissionRepository(TourCrmDbContext context)
    : GenericRepository<RolePermission>(context), IRolePermissionRepository
{
    public async Task<IEnumerable<RolePermission>> GetByRoleIdAsync(int roleId) =>
        await _context.RolePermissions
            .Where(rp => rp.RoleId == roleId)
            .ToListAsync();

    public async Task ReplaceForRoleAsync(int roleId, IEnumerable<RolePermission> items, CancellationToken ct = default)
    {
        var existing = await _context.RolePermissions
            .Where(rp => rp.RoleId == roleId)
            .ToListAsync(ct);

        _context.RolePermissions.RemoveRange(existing);
        await _context.RolePermissions.AddRangeAsync(items, ct);
    }

    public async Task AddRangeAsync(IEnumerable<RolePermission> entities) =>
        await _context.RolePermissions.AddRangeAsync(entities);
}