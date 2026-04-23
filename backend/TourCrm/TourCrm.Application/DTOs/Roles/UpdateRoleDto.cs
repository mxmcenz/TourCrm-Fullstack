namespace TourCrm.Application.DTOs.Roles;

public sealed record UpdateRoleDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public int CompanyId { get; init; }
}