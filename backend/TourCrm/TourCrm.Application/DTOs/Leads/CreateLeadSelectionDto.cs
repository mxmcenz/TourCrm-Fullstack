namespace TourCrm.Application.DTOs.Leads;

public class CreateLeadSelectionDto
{
    public string DepartureCity { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string? Hotel { get; set; }
    public string? RoomType { get; set; }
    public string Accommodation { get; set; } = string.Empty;
    public string MealPlan { get; set; } = string.Empty;
    public DateOnly? StartDate { get; set; }
    public int? Nights { get; set; }
    public int? Adults { get; set; }
    public int? Children { get; set; }
    public int? Infants { get; set; }
    public string? Link { get; set; }
    public string? Note { get; set; }
    public int? PartnerId { get; set; }
    public string? PartnerName { get; set; }
    public decimal? Price { get; set; }
    public string? Currency { get; set; }
}