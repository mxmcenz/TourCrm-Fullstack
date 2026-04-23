using TourCrm.Core.Entities.Dictionaries;

namespace TourCrm.Core.Entities.Leads;

public class Lead
{
    public int Id { get; set; }
    public string LeadNumber { get; set; } = string.Empty;

    public int LeadStatusId { get; set; }
    public LeadStatus? LeadStatus { get; set; }

    public int? RequestTypeId { get; set; }
    public LeadRequestType? RequestType { get; set; }

    public int? SourceId { get; set; }
    public LeadSource? Source { get; set; }

    public int? ManagerId { get; set; }
    public Employee? Manager { get; set; }
    public string? ManagerFullName { get; set; }

    public string CustomerType { get; set; } = "person";
    public string CustomerFirstName { get; set; } = string.Empty;
    public string CustomerLastName { get; set; } = string.Empty;
    public string? CustomerMiddleName { get; set; }

    public string? Phone { get; set; }
    public string? Email { get; set; }

    public string? Country { get; set; }
    public int? Adults { get; set; }
    public int? Children { get; set; }
    public int? Infants { get; set; }

    public DateOnly? DesiredArrival { get; set; }
    public DateOnly? DesiredDeparture { get; set; }
    public int? NightsFrom { get; set; }
    public int? NightsTo { get; set; }
    public decimal? Budget { get; set; }

    public string? Accommodation { get; set; }
    public string? MealPlan { get; set; }
    public string? Note { get; set; }

    public DateOnly? DocsPackageDate { get; set; }
    public bool PrecontractRequired { get; set; }
    public bool InvoiceRequired { get; set; }

    public string CreatedByUserId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }

    public int OfficeId { get; set; }
    public Office Office { get; set; } = null!;

    public int CompanyId { get; set; }
    public Company Company { get; set; } = null!;

    public ICollection<LeadLabel> LeadLabels { get; set; } = new List<LeadLabel>();
    public ICollection<LeadSelection> Selections { get; set; } = new List<LeadSelection>();
}