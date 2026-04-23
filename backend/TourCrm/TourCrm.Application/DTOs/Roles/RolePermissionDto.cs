namespace TourCrm.Application.DTOs.Roles;

public sealed record RolePermissionDto
{
    public string Key { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public bool IsGranted { get; init; }
}