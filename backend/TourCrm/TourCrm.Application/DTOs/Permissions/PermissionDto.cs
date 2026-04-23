namespace TourCrm.Application.DTOs.Permissions;

public sealed record PermissionDto
{
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}