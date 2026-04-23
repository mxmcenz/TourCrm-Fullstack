namespace TourCrm.Application.DTOs.Companies;

public class CompanyUpsertDto
{
    public string Name { get; set; } = string.Empty;
    public int? LegalEntityId { get; set; }
}