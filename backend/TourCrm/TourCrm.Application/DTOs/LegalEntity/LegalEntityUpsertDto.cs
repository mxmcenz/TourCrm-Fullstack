namespace TourCrm.Application.DTOs.LegalEntity;

public class LegalEntityUpsertDto
{
    public string? NameRu { get; set; }
    public string? NameEn { get; set; }
    public string? NameFull { get; set; }
    public int? CountryId { get; set; }
    public int? CityId { get; set; }
    public string? LegalAddress { get; set; }
    public string? ActualAddress { get; set; }
    public string? DirectorFio { get; set; }
    public string? DirectorFioGen { get; set; }
    public string? DirectorPost { get; set; } = "Директор";
    public string? DirectorPostGen { get; set; } = "Директора";
    public string? DirectorBasis { get; set; } = "Устава";
    public string? CfoFio { get; set; }
    public string? Phones { get; set; }
    public string? Website { get; set; }
    public string? Email { get; set; }
    public string? BinIin { get; set; }
    public string? BankDetailsJson { get; set; }
    public int? CompanyId { get; set; }
}