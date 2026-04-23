using TourCrm.Core.Entities.Dictionaries;

namespace TourCrm.Core.Entities;

public class LegalEntity
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? NameRu { get; set; }
    public string? NameEn { get; set; }
    public string? NameFull { get; set; }

    public int? CountryId { get; set; }
    public Country? CountryRef { get; set; } 

    public int? CityId { get; set; }
    public City? CityRef { get; set; }

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

    public int CompanyId { get; set; }
    public Company Company { get; set; } = null!;
    
    public string? Phone { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; }

    public ICollection<Office> Offices { get; set; } = new List<Office>();
}