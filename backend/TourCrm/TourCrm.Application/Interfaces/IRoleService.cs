using TourCrm.Application.Common.Results;
using TourCrm.Application.DTOs;
using TourCrm.Application.DTOs.Permissions;
using TourCrm.Application.DTOs.Roles;

namespace TourCrm.Application.Interfaces;

public interface IRoleService
{
    Task<ServiceResult<List<RoleDto>>> GetAllRolesAsync(string userId, CancellationToken ct = default);
    Task<ServiceResult<RoleDto?>> GetRoleByIdAsync(int id, string userId, CancellationToken ct = default);
    Task<ServiceResult<RoleDto>> CreateRoleAsync(CreateRoleDto role, string userId, CancellationToken ct = default);
    Task<ServiceResult<object>> UpdateRoleAsync(UpdateRoleDto role, string userId, CancellationToken ct = default);
    Task<ServiceResult<object>> DeleteRoleAsync(int id, string userId, CancellationToken ct = default);

    Task<ServiceResult<List<RolePermissionDto>>> GetRolePermissionsAsync(int roleId, string userId,
        CancellationToken ct = default);

    Task<ServiceResult<object>> SetRolePermissionsAsync(int roleId, string userId, GrantPermissionsDto dto,
        CancellationToken ct = default);

    Task<ServiceResult<object>> AssignRoleToUserAsync(int userId, int roleId, string callerUserId,
        CancellationToken ct = default);

    Task<ServiceResult<object>> RemoveRoleFromUserAsync(int userId, int roleId, string callerUserId,
        CancellationToken ct = default);

    Task<ServiceResult<List<RoleDto>>> GetUserRolesAsync(int userId, string callerUserId,
        CancellationToken ct = default);

    Task<ServiceResult<PagedResult<RoleDto>>> GetRolesPagedAsync(RolesQuery q, string userId,
        CancellationToken ct = default);
}