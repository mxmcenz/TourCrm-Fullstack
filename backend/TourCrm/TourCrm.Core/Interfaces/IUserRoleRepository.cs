using TourCrm.Core.Entities;
using TourCrm.Core.Entities.Roles;

namespace TourCrm.Core.Interfaces;

public interface IUserRoleRepository : IGenericRepository<UserRole>
{
    Task<bool> ExistsAsync(int userId, int roleId);
    Task<List<string>> GetRolesByUserIdAsync(int userId);
    Task<List<string>> GetPermissionsByUserIdAsync(int userId);
}