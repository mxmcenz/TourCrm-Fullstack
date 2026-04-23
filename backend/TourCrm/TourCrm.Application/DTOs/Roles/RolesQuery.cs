namespace TourCrm.Application.DTOs.Roles;

public sealed class RolesQuery
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? Search { get; init; }
    public string SortBy { get; init; } = "name"; 
    public bool Desc { get; init; } = false;
}