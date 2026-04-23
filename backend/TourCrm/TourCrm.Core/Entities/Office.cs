using TourCrm.Core.Entities.Deals;
using TourCrm.Core.Entities.Leads;

namespace TourCrm.Core.Entities;

public class Office
{
    public int Id { get; set; }

    public int LegalEntityId { get; set; }
    public LegalEntity LegalEntity { get; set; } = null!;

    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; } 
    public int? LeadLimit { get; set; }  

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; }

    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    public ICollection<Lead> Leads { get; set; } = new List<Lead>();
    public ICollection<Deal> Deals { get; set; } = new List<Deal>();
}