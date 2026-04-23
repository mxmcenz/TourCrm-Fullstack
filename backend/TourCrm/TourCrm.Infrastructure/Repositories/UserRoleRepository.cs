using Microsoft.EntityFrameworkCore;
using TourCrm.Application.Interfaces;
using TourCrm.Core.Entities;
using TourCrm.Core.Entities.Roles;
using TourCrm.Core.Interfaces;
using TourCrm.Infrastructure.Data;

namespace TourCrm.Infrastructure.Repositories;

public class UserRoleRepository(TourCrmDbContext context, IPermissionProvider permissionProvider)
    : GenericRepository<UserRole>(context), IUserRoleRepository
{
    public Task<bool> ExistsAsync(int userId, int roleId) =>
        _context.UserRoles.AnyAsync(ur => ur.UserId == userId && ur.RoleId == roleId);

    public Task<List<string>> GetRolesByUserIdAsync(int userId) =>
        _context.UserRoles
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.Role.Name)
            .ToListAsync();

    public async Task<List<string>> GetPermissionsByUserIdAsync(int userId)
    {
        var roleIds = await _context.UserRoles
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.RoleId)
            .ToListAsync();

        if (!roleIds.Any()) return new List<string>();

        var rolePermissionKeys = await _context.RolePermissions
            .Where(rp => roleIds.Contains(rp.RoleId) && rp.IsGranted)
            .Select(rp => rp.PermissionKey)
            .ToListAsync();

        var allPermissions = await permissionProvider.GetPermissionsAsync();
        return allPermissions
            .Where(p => rolePermissionKeys.Contains(p.Key))
            .Select(p => p.Key)
            .ToList();
    }
}