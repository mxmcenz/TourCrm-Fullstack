namespace TourCrm.Application.DTOs.Roles;

public sealed record CreateRoleDto
{
    public string Name { get; init; } = string.Empty;
    public int? CompanyId { get; init; }
}