using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TourCrm.Application.DTOs;
using TourCrm.Application.DTOs.Permissions;
using TourCrm.Application.Interfaces;
using TourCrm.Application.Settings;
using TourCrm.Core.Entities;
using TourCrm.Core.Entities.Roles;

namespace TourCrm.Infrastructure.Data;

public class DbInitializer(
    TourCrmDbContext context,
    IPasswordHasher passwordHasher,
    IOptions<SuperAdminSettings> adminSettings,
    IPermissionProvider permissionProvider)
{
    private readonly SuperAdminSettings _adminSettings = adminSettings.Value;
    private const string SystemCompanyName = "__SYSTEM__";
    private const string SuperAdminRoleName = "SuperAdmin";

    public async Task InitializeAsync()
    {
        await context.Database.MigrateAsync();

        var permissions = await permissionProvider.GetPermissionsAsync();

        await SyncRolePermissionsAsync(permissions);

        var superAdminUser = await EnsureSuperAdminUserAsync();
        var systemCompanyId = await EnsureSystemCompanyForSuperAdminAsync(superAdminUser.Id.ToString());
        var superAdminRoleId = await EnsureSuperAdminRoleAsync(permissions, systemCompanyId);

        await EnsureUserRoleLinkAsync(superAdminUser.Id, superAdminRoleId);
    }

    private async Task SyncRolePermissionsAsync(IReadOnlyList<PermissionDto> permissions)
    {
        var roles = await context.Roles.Include(r => r.RolePermissions).ToListAsync();

        foreach (var role in roles)
        {
            var existingKeys = role.RolePermissions.Select(rp => rp.PermissionKey).ToHashSet();

            var missingPermissions = permissions
                .Where(p => !existingKeys.Contains(p.Key))
                .Select(p => new RolePermission
                {
                    RoleId = role.Id,
                    PermissionKey = p.Key,
                    IsGranted = role.Name == SuperAdminRoleName
                })
                .ToList();

            if (missingPermissions.Any())
                context.RolePermissions.AddRange(missingPermissions);
        }

        await context.SaveChangesAsync();
    }

    private async Task<int> EnsureSystemCompanyForSuperAdminAsync(string ownerUserId)
    {
        var company = await context.Companies.FirstOrDefaultAsync(c => c.OwnerUserId == ownerUserId)
                      ?? await context.Companies.FirstOrDefaultAsync(c => c.Name == SystemCompanyName);

        if (company == null)
        {
            company = new Company
            {
                Name = SystemCompanyName,
                OwnerUserId = ownerUserId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            context.Companies.Add(company);
            await context.SaveChangesAsync();
        }
        else if (company.OwnerUserId != ownerUserId)
        {
            company.OwnerUserId = ownerUserId;
            company.UpdatedAt = DateTime.UtcNow;
            await context.SaveChangesAsync();
        }

        return company.Id;
    }

    private async Task<int> EnsureSuperAdminRoleAsync(IReadOnlyList<PermissionDto> permissions, int companyId)
    {
        var role = await context.Roles.FirstOrDefaultAsync(r => r.Name == SuperAdminRoleName);

        if (role == null)
        {
            role = new Role { Name = SuperAdminRoleName, CompanyId = companyId };
            context.Roles.Add(role);
            await context.SaveChangesAsync();

            var superAdminPermissions = permissions.Select(p => new RolePermission
            {
                RoleId = role.Id,
                PermissionKey = p.Key,
                IsGranted = true
            });

            context.RolePermissions.AddRange(superAdminPermissions);
            await context.SaveChangesAsync();
        }
        else if (role.CompanyId <= 0)
        {
            role.CompanyId = companyId;
            await context.SaveChangesAsync();
        }

        return role.Id;
    }

    private async Task<User> EnsureSuperAdminUserAsync()
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Email == _adminSettings.Email);
        if (user == null)
        {
            user = new User
            {
                Email = _adminSettings.Email,
                PhoneNumber = "+77000000000",
                PasswordHash = passwordHasher.Hash(_adminSettings.Password),
                IsEmailConfirmed = true,
                FirstName = "Super",
                LastName = "Admin"
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();
        }

        return user;
    }

    private async Task EnsureUserRoleLinkAsync(int userId, int roleId)
    {
        var exists = await context.UserRoles.AnyAsync(ur => ur.UserId == userId && ur.RoleId == roleId);
        if (!exists)
        {
            context.UserRoles.Add(new UserRole { UserId = userId, RoleId = roleId });
            await context.SaveChangesAsync();
        }
    }
}