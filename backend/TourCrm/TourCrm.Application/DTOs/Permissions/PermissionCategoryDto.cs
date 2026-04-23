namespace TourCrm.Application.DTOs.Permissions;

public class PermissionCategoryDto
{
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public List<PermissionDto> Items { get; set; } = new();
}