namespace TourCrm.Application.DTOs.Offices;

public class OfficeListItemDto
{
    public int Id { get; set; }
    public int LegalEntityId { get; set; }
    public string? LegalEntityName { get; set; }

    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public int? LeadLimit { get; set; }

    public int EmployeesCount { get; set; }
    public DateTime CreatedAt { get; set; }
}