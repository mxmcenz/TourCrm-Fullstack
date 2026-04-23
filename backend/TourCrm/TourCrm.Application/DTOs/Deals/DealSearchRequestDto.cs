namespace TourCrm.Application.DTOs.Deals;

public class DealSearchRequestDto
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    public string? Query { get; set; }
    public string? Scope { get; set; }    

    public string? SortBy { get; set; } = "createdAt";
    public string? SortDir { get; set; } = "desc";

    public string? DealNumber { get; set; }
    public string? Booking { get; set; }
    public int?    StatusId { get; set; }
    public int?    ManagerId { get; set; }

    public DateTime? CreatedFrom { get; set; }
    public DateTime? CreatedTo   { get; set; }
    public DateTime? StartFrom   { get; set; }
    public DateTime? StartTo     { get; set; }
    public DateTime? EndFrom     { get; set; }
    public DateTime? EndTo       { get; set; }

    public DateTime? ClientPaymentFrom  { get; set; }
    public DateTime? ClientPaymentTo    { get; set; }
    public DateTime? PartnerPaymentFrom { get; set; }
    public DateTime? PartnerPaymentTo   { get; set; }

    public string? Tourist { get; set; }
    public decimal? PriceFrom { get; set; }
    public decimal? PriceTo   { get; set; }
}