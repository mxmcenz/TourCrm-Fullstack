namespace TourCrm.Core.Entities.Roles;

public class Role
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? CompanyId { get; set; }
    public Company? Company { get; set; } = null!;
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}