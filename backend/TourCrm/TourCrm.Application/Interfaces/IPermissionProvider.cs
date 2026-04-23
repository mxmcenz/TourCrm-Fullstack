using TourCrm.Application.DTOs;
using TourCrm.Application.DTOs.Permissions;

namespace TourCrm.Application.Interfaces;

public interface IPermissionProvider
{
    Task<IReadOnlyList<PermissionDto>> GetPermissionsAsync();
    Task<IReadOnlyList<PermissionCategoryDto>> GetPermissionTreeAsync();
}