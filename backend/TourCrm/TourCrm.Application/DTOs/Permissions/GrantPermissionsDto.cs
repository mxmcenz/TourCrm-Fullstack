namespace TourCrm.Application.DTOs.Permissions;

public sealed record GrantPermissionsDto
{
    public List<string> Keys { get; init; } = new();
}