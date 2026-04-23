namespace TourCrm.Application.DTOs.LegalEntity;

public class LegalEntityDto
{
    public int Id { get; set; }

    public string? NameRu { get; set; }
    public string? NameEn { get; set; }
    public string? NameFull { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? CountryId { get; set; }
    public string? CountryName { get; set; }
    public int? CityId { get; set; }
    public string? CityName { get; set; }
    public string? LegalAddress { get; set; }
    public string? ActualAddress { get; set; }

    public string? DirectorFio { get; set; }
    public string? DirectorFioGen { get; set; }
    public string? DirectorPost { get; set; }
    public string? DirectorPostGen { get; set; }
    public string? DirectorBasis { get; set; }

    public string? CfoFio { get; set; }

    public string? Phones { get; set; }
    public string? Website { get; set; }
    public string? Email { get; set; }
    public string? BinIin { get; set; }

    public string? BankDetailsJson { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}