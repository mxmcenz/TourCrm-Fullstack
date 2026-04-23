namespace TourCrm.Application.DTOs.Leads;

public sealed record CreateLeadDto
{
    public int LeadStatusId { get; set; } = 1;
    public int? RequestTypeId { get; init; }
    public int? SourceId { get; init; }

    public int? ManagerId { get; init; }
    public string? ManagerFullName { get; init; }

    public string CustomerType { get; init; } = "person";
    public string CustomerFirstName { get; init; } = string.Empty;
    public string CustomerLastName  { get; init; } = string.Empty;
    public string? CustomerMiddleName { get; init; }

    public string? Phone { get; init; }
    public string? Email { get; init; }

    public List<int>? LabelIds { get; init; }

    public string? Country { get; init; }
    public int? Adults { get; init; }
    public int? Children { get; init; }
    public int? Infants { get; init; }

    public DateOnly? DesiredArrival { get; init; }
    public DateOnly? DesiredDeparture { get; init; }
    public int? NightsFrom { get; init; }
    public int? NightsTo { get; init; }
    public decimal? Budget { get; init; }

    public string? Accommodation { get; init; }
    public string? MealPlan { get; init; }
    public string? Note { get; init; }

    public DateOnly? DocsPackageDate { get; init; }
    public bool PrecontractRequired { get; init; }
    public bool InvoiceRequired { get; init; }
}