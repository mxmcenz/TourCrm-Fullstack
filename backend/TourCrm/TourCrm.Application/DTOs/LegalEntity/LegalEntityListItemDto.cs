namespace TourCrm.Application.DTOs.LegalEntity;

public class LegalEntityListItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty; 
    public int? CountryId { get; set; }
    public string? CountryName { get; set; }
    public int? CityId { get; set; }
    public string? CityName { get; set; }
    public string? Phone { get; set; }
    public List<string>? Phones { get; set; }
    public string? PrimaryEmail { get; set; }
    public int EmployeesCount { get; set; }
    public DateTime CreatedAt { get; set; }
}