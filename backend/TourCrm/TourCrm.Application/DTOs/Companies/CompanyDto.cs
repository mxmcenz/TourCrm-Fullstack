namespace TourCrm.Application.DTOs.Companies;

public class CompanyDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public int? LegalEntityId { get; set; }
    public string? LegalEntityName { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}