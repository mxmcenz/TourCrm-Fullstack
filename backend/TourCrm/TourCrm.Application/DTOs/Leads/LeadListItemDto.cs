namespace TourCrm.Application.DTOs.Leads;

public sealed record LeadListItemDto
{
    public int Id { get; init; }
    public string LeadNumber { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateOnly? TravelDate { get; init; }
    public int? Nights { get; init; }   
    public string? Country { get; init; }
    public decimal? Budget { get; init; }
    public string? Manager { get; init; }
}