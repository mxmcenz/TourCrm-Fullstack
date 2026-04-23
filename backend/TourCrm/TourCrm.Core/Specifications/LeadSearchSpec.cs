namespace TourCrm.Core.Specifications;

public sealed class LeadSearchSpec
{
    public int? StatusId { get; set; }
    public DateTime? CreatedFrom { get; set; }
    public DateTime? CreatedTo { get; set; }
    public DateOnly? TravelFrom { get; set; }
    public DateOnly? TravelTo { get; set; }
    public int? NightsMin { get; set; }
    public int? NightsMax { get; set; }
    public string? Country { get; set; }
    public decimal? BudgetMin { get; set; }
    public decimal? BudgetMax { get; set; }
    public int? ManagerId { get; set; }
    public int? OfficeId { get; set; }
    public string? Query { get; set; }

    public string? SortBy { get; set; }
    public string? SortDir { get; set; } 

    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}