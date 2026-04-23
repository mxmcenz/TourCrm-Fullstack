namespace TourCrm.Application.DTOs.Deals;

public record ClientShortDto
{
    public int Id { get; init; }
    public string FullName { get; init; } = "";
    public string? Phone { get; init; }
    public string? Email { get; init; }
}