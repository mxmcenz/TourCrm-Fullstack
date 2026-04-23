using TourCrm.Application.Common.Results;
using TourCrm.Application.DTOs;
using TourCrm.Application.DTOs.Permissions;
using TourCrm.Application.DTOs.Roles;
using TourCrm.Application.Interfaces;
using TourCrm.Core.Entities;
using TourCrm.Core.Entities.Roles;
using TourCrm.Core.Interfaces;

namespace TourCrm.Application.Services;

public class RoleService(
    IUnitOfWork unitOfWork,
    IPermissionProvider permissionProvider,
    ICompanyRepository companyRepo
) : IRoleService
{
    private const string SuperAdminRoleName = "SuperAdmin";

    private async Task<bool> IsSuperAdminAsync(string userId, CancellationToken ct)
    {
        if (!int.TryParse(userId, out var uid)) return false;

        var userRoles = (await unitOfWork.UserRoles.GetAllAsync(ct)).ToList();
        if (userRoles.Count == 0) return false;

        var roleIds = userRoles.Where(ur => ur.UserId == uid).Select(ur => ur.RoleId).ToHashSet();
        if (roleIds.Count == 0) return false;

        var roles = (await unitOfWork.Roles.GetAllAsync(ct)).ToList();
        return roles.Any(r =>
            roleIds.Contains(r.Id) && r.Name.Equals(SuperAdminRoleName, StringComparison.OrdinalIgnoreCase));
    }

    private async Task<int> GetMyCompanyIdOrThrowAsync(string userId, CancellationToken ct)
    {
        var company = await companyRepo.GetByOwnerAsync(userId, ct);
        if (company is not null) return company.Id;

        if (!int.TryParse(userId, out var uid))
            throw new InvalidOperationException("Некорректный идентификатор пользователя.");

        var me = await unitOfWork.Employees.GetByIdAsync(uid);
        if (me is null)
            throw new UnauthorizedAccessException("Пользователь не найден.");
        if (me.LegalEntityId <= 0)
            throw new InvalidOperationException("Пользователь не привязан к юрлицу.");

        var companyByLe = await companyRepo.GetByLegalEntityIdAsync(me.LegalEntityId, ct)
                          ?? throw new InvalidOperationException("Компания для указанного юрлица не найдена.");

        return companyByLe.Id;
    }

    public async Task<ServiceResult<List<RoleDto>>> GetAllRolesAsync(string userId, CancellationToken ct = default)
    {
        var isSa = await IsSuperAdminAsync(userId, ct);
        var roles = (await unitOfWork.Roles.GetAllAsync(ct)).ToList();

        IEnumerable<Role> filtered =
            roles.Where(r => !r.Name.Equals(SuperAdminRoleName, StringComparison.OrdinalIgnoreCase));
        if (!isSa)
        {
            var companyId = await GetMyCompanyIdOrThrowAsync(userId, ct);
            filtered = filtered.Where(r => r.CompanyId == companyId);
        }

        var rolesDto = filtered.Select(r => new RoleDto { Id = r.Id, Name = r.Name, CompanyId = r.CompanyId }).ToList();
        return ServiceResult<List<RoleDto>>.Ok(rolesDto);
    }

    public async Task<ServiceResult<RoleDto?>> GetRoleByIdAsync(int id, string userId, CancellationToken ct = default)
    {
        var isSa = await IsSuperAdminAsync(userId, ct);
        var role = await unitOfWork.Roles.GetByIdAsync(id, ct);
        if (role == null || role.Name.Equals(SuperAdminRoleName, StringComparison.OrdinalIgnoreCase))
            return ServiceResult<RoleDto?>.Fail("Роль не найдена");
        if (!isSa)
        {
            var companyId = await GetMyCompanyIdOrThrowAsync(userId, ct);
            if (role.CompanyId != companyId) return ServiceResult<RoleDto?>.Fail("Роль не найдена");
        }

        return ServiceResult<RoleDto?>.Ok(new RoleDto { Id = role.Id, Name = role.Name, CompanyId = role.CompanyId });
    }

    public async Task<ServiceResult<RoleDto>> CreateRoleAsync(CreateRoleDto dto, string userId,
        CancellationToken ct = default)
    {
        if (dto.Name.Equals(SuperAdminRoleName, StringComparison.OrdinalIgnoreCase))
            return ServiceResult<RoleDto>.Fail("Создание этой роли запрещено");

        var isSa = await IsSuperAdminAsync(userId, ct);

        int companyId;
        if (isSa)
        {
            if (dto.CompanyId is null || dto.CompanyId <= 0) return ServiceResult<RoleDto>.Fail("Не указан CompanyId");
            companyId = dto.CompanyId.Value;
        }
        else
        {
            companyId = await GetMyCompanyIdOrThrowAsync(userId, ct);
        }

        var role = new Role { Name = dto.Name.Trim(), CompanyId = companyId };
        await unitOfWork.Roles.AddAsync(role, ct);
        await unitOfWork.SaveChangesAsync(ct);

        var allPermissions = await permissionProvider.GetPermissionsAsync();
        var rolePermissions = allPermissions.Select(p => new RolePermission
            { RoleId = role.Id, PermissionKey = p.Key, IsGranted = false }).ToList();
        await unitOfWork.RolePermissions.AddRangeAsync(rolePermissions);
        await unitOfWork.SaveChangesAsync(ct);

        return ServiceResult<RoleDto>.Ok(new RoleDto { Id = role.Id, Name = role.Name, CompanyId = role.CompanyId },
            "Роль успешно создана");
    }

    public async Task<ServiceResult<object>> UpdateRoleAsync(UpdateRoleDto dto, string userId,
        CancellationToken ct = default)
    {
        var isSa = await IsSuperAdminAsync(userId, ct);
        var role = await unitOfWork.Roles.GetByIdAsync(dto.Id, ct);
        if (role == null || role.Name.Equals(SuperAdminRoleName, StringComparison.OrdinalIgnoreCase))
            return ServiceResult<object>.Fail("Изменение этой роли запрещено");
        if (!isSa)
        {
            var companyId = await GetMyCompanyIdOrThrowAsync(userId, ct);
            if (role.CompanyId != companyId) return ServiceResult<object>.Fail("Изменение этой роли запрещено");
        }

        if (dto.Name.Equals(SuperAdminRoleName, StringComparison.OrdinalIgnoreCase))
            return ServiceResult<object>.Fail("Переименование в SuperAdmin запрещено");

        role.Name = dto.Name.Trim();
        unitOfWork.Roles.Update(role);
        await unitOfWork.SaveChangesAsync(ct);

        return ServiceResult<object>.Ok("Роль успешно обновлена");
    }

    public async Task<ServiceResult<object>> DeleteRoleAsync(int id, string userId, CancellationToken ct = default)
    {
        var isSa = await IsSuperAdminAsync(userId, ct);
        var role = await unitOfWork.Roles.GetByIdAsync(id, ct);
        if (role == null || role.Name.Equals(SuperAdminRoleName, StringComparison.OrdinalIgnoreCase))
            return ServiceResult<object>.Fail("Удаление этой роли запрещено");
        if (!isSa)
        {
            var companyId = await GetMyCompanyIdOrThrowAsync(userId, ct);
            if (role.CompanyId != companyId) return ServiceResult<object>.Fail("Удаление этой роли запрещено");
        }

        unitOfWork.Roles.Delete(role);
        await unitOfWork.SaveChangesAsync(ct);

        return ServiceResult<object>.Ok("Роль успешно удалена");
    }

    public async Task<ServiceResult<List<RolePermissionDto>>> GetRolePermissionsAsync(int roleId, string userId,
        CancellationToken ct = default)
    {
        var isSa = await IsSuperAdminAsync(userId, ct);
        var role = await unitOfWork.Roles.GetByIdAsync(roleId, ct);
        if (role == null || role.Name.Equals(SuperAdminRoleName, StringComparison.OrdinalIgnoreCase))
            return ServiceResult<List<RolePermissionDto>>.Fail("Роль не найдена");
        if (!isSa)
        {
            var companyId = await GetMyCompanyIdOrThrowAsync(userId, ct);
            if (role.CompanyId != companyId) return ServiceResult<List<RolePermissionDto>>.Fail("Роль не найдена");
        }

        var allPermissions = await permissionProvider.GetPermissionsAsync();
        var rolePermissions = (await unitOfWork.RolePermissions.GetByRoleIdAsync(roleId)).ToList();
        var map = rolePermissions.ToDictionary(rp => rp.PermissionKey, rp => rp.IsGranted);

        var result = allPermissions
            .Select(p => new RolePermissionDto
                { Key = p.Key, Name = p.Name, IsGranted = map.TryGetValue(p.Key, out var g) && g })
            .ToList();

        return ServiceResult<List<RolePermissionDto>>.Ok(result);
    }

    public async Task<ServiceResult<object>> SetRolePermissionsAsync(int roleId, string userId, GrantPermissionsDto dto,
        CancellationToken ct = default)
    {
        var isSa = await IsSuperAdminAsync(userId, ct);
        var role = await unitOfWork.Roles.GetByIdAsync(roleId, ct);
        if (role == null || role.Name.Equals(SuperAdminRoleName, StringComparison.OrdinalIgnoreCase))
            return ServiceResult<object>.Fail("Изменение прав этой роли запрещено");
        if (!isSa)
        {
            var companyId = await GetMyCompanyIdOrThrowAsync(userId, ct);
            if (role.CompanyId != companyId) return ServiceResult<object>.Fail("Изменение прав этой роли запрещено");
        }

        var allowedKeys = (await permissionProvider.GetPermissionsAsync()).Select(p => p.Key).ToHashSet();
        var requested = dto.Keys.ToHashSet();
        if (!requested.IsSubsetOf(allowedKeys))
            return ServiceResult<object>.Fail(
                $"Неверные ключи доступа: {string.Join(", ", requested.Where(k => !allowedKeys.Contains(k)))}");

        var current = (await unitOfWork.RolePermissions.GetByRoleIdAsync(roleId)).ToList();
        foreach (var rp in current) rp.IsGranted = requested.Contains(rp.PermissionKey);

        await unitOfWork.SaveChangesAsync(ct);
        return ServiceResult<object>.Ok("Права роли успешно обновлены");
    }

    public async Task<ServiceResult<object>> AssignRoleToUserAsync(int userId, int roleId, string callerUserId,
        CancellationToken ct = default)
    {
        var isSa = await IsSuperAdminAsync(callerUserId, ct);

        var user = await unitOfWork.Users.GetByIdAsync(userId, ct);
        if (user == null) return ServiceResult<object>.Fail("Пользователь не найден");

        var role = await unitOfWork.Roles.GetByIdAsync(roleId, ct);
        if (role == null || role.Name.Equals(SuperAdminRoleName, StringComparison.OrdinalIgnoreCase))
            return ServiceResult<object>.Fail("Назначение этой роли запрещено");

        if (!isSa)
        {
            var companyId = await GetMyCompanyIdOrThrowAsync(callerUserId, ct);
            if (role.CompanyId != companyId) return ServiceResult<object>.Fail("Назначение этой роли запрещено");
        }

        var hasRole = await unitOfWork.UserRoles.ExistsAsync(userId, roleId);
        if (!hasRole)
        {
            await unitOfWork.UserRoles.AddAsync(new UserRole { UserId = userId, RoleId = roleId }, ct);
            await unitOfWork.SaveChangesAsync(ct);
        }

        return ServiceResult<object>.Ok("Роль успешно назначена пользователю");
    }

    public async Task<ServiceResult<object>> RemoveRoleFromUserAsync(int userId, int roleId, string callerUserId,
        CancellationToken ct = default)
    {
        var isSa = await IsSuperAdminAsync(callerUserId, ct);

        var user = await unitOfWork.Users.GetByIdAsync(userId, ct);
        if (user == null) return ServiceResult<object>.Fail("Пользователь не найден");

        var role = await unitOfWork.Roles.GetByIdAsync(roleId, ct);
        if (role == null || role.Name.Equals(SuperAdminRoleName, StringComparison.OrdinalIgnoreCase))
            return ServiceResult<object>.Fail("Удаление этой роли запрещено");

        if (!isSa)
        {
            var companyId = await GetMyCompanyIdOrThrowAsync(callerUserId, ct);
            if (role.CompanyId != companyId) return ServiceResult<object>.Fail("Удаление этой роли запрещено");
        }

        var hasRole = await unitOfWork.UserRoles.ExistsAsync(userId, roleId);
        if (!hasRole) return ServiceResult<object>.Fail("У пользователя нет этой роли");

        var allUserRoles = (await unitOfWork.UserRoles.GetAllAsync(ct)).ToList();
        var userRole = allUserRoles.FirstOrDefault(ur => ur.UserId == userId && ur.RoleId == roleId);
        if (userRole != null)
        {
            unitOfWork.UserRoles.Delete(userRole);
            await unitOfWork.SaveChangesAsync(ct);
        }

        return ServiceResult<object>.Ok("Роль успешно удалена у пользователя");
    }

    public async Task<ServiceResult<List<RoleDto>>> GetUserRolesAsync(int userId, string callerUserId,
        CancellationToken ct = default)
    {
        var isSa = await IsSuperAdminAsync(callerUserId, ct);

        var user = await unitOfWork.Users.GetByIdAsync(userId, ct);
        if (user == null) return ServiceResult<List<RoleDto>>.Fail("Пользователь не найден");

        var roleIds = (await unitOfWork.UserRoles.GetAllAsync(ct))
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.RoleId)
            .ToHashSet();

        if (roleIds.Count == 0) return ServiceResult<List<RoleDto>>.Ok(new List<RoleDto>());

        var rolesAll = (await unitOfWork.Roles.GetAllAsync(ct)).ToList();
        IEnumerable<Role> roles = rolesAll.Where(r =>
            roleIds.Contains(r.Id) && !r.Name.Equals(SuperAdminRoleName, StringComparison.OrdinalIgnoreCase));
        if (!isSa)
        {
            var companyId = await GetMyCompanyIdOrThrowAsync(callerUserId, ct);
            roles = roles.Where(r => r.CompanyId == companyId);
        }

        var dtos = roles.Select(r => new RoleDto { Id = r.Id, Name = r.Name, CompanyId = r.CompanyId }).ToList();
        return ServiceResult<List<RoleDto>>.Ok(dtos);
    }

    public async Task<ServiceResult<PagedResult<RoleDto>>> GetRolesPagedAsync(RolesQuery q, string userId,
        CancellationToken ct = default)
    {
        if (q.Page <= 0 || q.PageSize <= 0)
            return ServiceResult<PagedResult<RoleDto>>.Fail("Некорректные параметры пагинации");

        var isSa = await IsSuperAdminAsync(userId, ct);

        if (!isSa)
        {
            var companyId = await GetMyCompanyIdOrThrowAsync(userId, ct);
            var (items, total) = await unitOfWork.Roles.GetPagedAsync(q.Page, q.PageSize, q.Search, q.SortBy, q.Desc,
                SuperAdminRoleName, companyId);
            var dtos = items.Select(r => new RoleDto { Id = r.Id, Name = r.Name, CompanyId = r.CompanyId }).ToList();
            var result = new PagedResult<RoleDto>
                { Items = dtos, TotalCount = total, Page = q.Page, PageSize = q.PageSize };
            return ServiceResult<PagedResult<RoleDto>>.Ok(result);
        }

        var all = (await unitOfWork.Roles.GetAllAsync(ct))
            .Where(r => !r.Name.Equals(SuperAdminRoleName, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(q.Search))
        {
            var term = q.Search.Trim();
            all = all.Where(r => r.Name.Contains(term, StringComparison.OrdinalIgnoreCase));
        }

        all = (q.SortBy?.ToLowerInvariant()) switch
        {
            "name" => q.Desc ? all.OrderByDescending(r => r.Name) : all.OrderBy(r => r.Name),
            "companyid" => q.Desc ? all.OrderByDescending(r => r.CompanyId) : all.OrderBy(r => r.CompanyId),
            _ => q.Desc ? all.OrderByDescending(r => r.Id) : all.OrderBy(r => r.Id)
        };

        var allList = all.ToList();
        var totalSa = allList.Count;
        var pageItems = allList.Skip((q.Page - 1) * q.PageSize).Take(q.PageSize).ToList();
        var pageDtos = pageItems.Select(r => new RoleDto { Id = r.Id, Name = r.Name, CompanyId = r.CompanyId })
            .ToList();

        var saResult = new PagedResult<RoleDto>
            { Items = pageDtos, TotalCount = totalSa, Page = q.Page, PageSize = q.PageSize };
        return ServiceResult<PagedResult<RoleDto>>.Ok(saResult);
    }
}