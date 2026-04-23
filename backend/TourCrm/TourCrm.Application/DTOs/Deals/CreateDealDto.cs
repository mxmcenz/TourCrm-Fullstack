using TourCrm.Application.DTOs.Deals.FiledsForCreate;

namespace TourCrm.Application.DTOs.Deals;

public record CreateDealDto
{
    public string? DealNumber { get; init; }
    public string? InternalNumber { get; init; }
    public int? StatusId { get; init; }
    public int? LeadId { get; init; }
    public int ManagerId { get; init; }
    public int CompanyId { get; init; }
    public int? IssuerLegalEntityId { get; init; }
    public int? RequestTypeId { get; init; }
    public int? SourceId { get; init; }
    public int? TourOperatorId { get; init; }
    public int? VisaTypeId { get; init; }
    public string TourName { get; init; } = "";
    public decimal Price { get; init; }
    public string? BookingNumbers { get; init; }
    public string? Note { get; init; }
    public DateOnly? StartDate { get; init; }
    public DateOnly? EndDate { get; init; }
    public DateOnly? ClientPaymentDeadline { get; init; }
    public DateOnly? PartnerPaymentDeadline { get; init; }
    public DateOnly? DocsPackageDate { get; init; }
    public bool AddStampAndSign { get; init; }
    public bool IncludeCostCalc { get; init; }
    public bool IncludeTourCalc { get; init; }
    public List<NewCustomerDto> Customers { get; init; } = [];
    public List<NewTouristDto> Tourists { get; init; } = [];
    public List<NewServiceDto> Services { get; init; } = [];
    public List<NewPaymentDto> ClientPays { get; init; } = [];
    public List<NewPaymentDto> PartnerPays { get; init; } = [];
}