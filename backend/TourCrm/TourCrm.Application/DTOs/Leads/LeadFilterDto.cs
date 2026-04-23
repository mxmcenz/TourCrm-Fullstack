namespace TourCrm.Application.DTOs.Leads;

public sealed record LeadFilterDto
{
    public int? StatusId { get; init; }
    public DateTime? CreatedFrom { get; init; }
    public DateTime? CreatedTo { get; init; }
    public DateOnly? TravelFrom { get; init; }
    public DateOnly? TravelTo { get; init; }
    public int? NightsMin { get; init; }
    public int? NightsMax { get; init; }
    public string? Country { get; init; }
    public decimal? BudgetMin { get; init; }
    public decimal? BudgetMax { get; init; }
    public int? ManagerId { get; init; }
    public int? OfficeId { get; init; }
    public string? Query { get; init; }

    public string? SortBy { get; init; }
    public string? SortDir { get; init; } 

    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}