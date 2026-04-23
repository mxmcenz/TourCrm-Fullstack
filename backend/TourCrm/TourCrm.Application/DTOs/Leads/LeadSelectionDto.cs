namespace TourCrm.Application.DTOs.Leads;

public sealed class LeadSelectionDto : CreateLeadSelectionDto
{
    public int Id { get; set; }
    public int LeadId { get; set; }
    public DateTime CreatedAt { get; set; }
}