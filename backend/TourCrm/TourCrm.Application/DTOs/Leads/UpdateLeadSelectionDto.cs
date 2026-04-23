namespace TourCrm.Application.DTOs.Leads;

public sealed class UpdateLeadSelectionDto
{
    public string DepartureCity { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public string? Hotel { get; init; }
    public string? RoomType { get; init; }
    public string Accommodation { get; init; } = string.Empty;
    public string MealPlan { get; init; } = string.Empty;
    public DateOnly? StartDate { get; init; }
    public int? Nights { get; init; }
    public int Adults { get; init; }
    public int Children { get; init; }
    public int Infants { get; init; }
    public string? Link { get; init; }
    public string? Note { get; init; }
    public int? PartnerId { get; init; }
    public string? PartnerName { get; init; } 
    public decimal? Price { get; init; }
    public string? Currency { get; init; }
}