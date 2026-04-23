using TourCrm.Core.Entities.Roles;
using TourCrm.Core.Entities.Tariffs;

namespace TourCrm.Core.Entities;

public class Company
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public int? LegalEntityId { get; set; }
    public LegalEntity? LegalEntity { get; set; }

    public string OwnerUserId { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public ICollection<LegalEntity> LegalEntities { get; set; } = new List<LegalEntity>();
    public ICollection<Role> Roles { get; set; } = new List<Role>();
    public int? TariffId { get; set; }
    public Tariff? Tariff { get; set; }
    public DateTime TariffExpiresAt { get; set; }
}