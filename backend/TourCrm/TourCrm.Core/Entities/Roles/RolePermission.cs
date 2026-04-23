namespace TourCrm.Core.Entities.Roles;

public class RolePermission
{
    public int RoleId { get; set; }
    public Role Role { get; set; } = null!;
    public string PermissionKey { get; set; } = string.Empty;
    public bool IsGranted { get; set; }
}