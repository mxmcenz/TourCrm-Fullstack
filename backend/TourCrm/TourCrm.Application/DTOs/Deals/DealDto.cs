using TourCrm.Application.DTOs.Deals.FiledsForCreate;

namespace TourCrm.Application.DTOs.Deals;

public record DealDto
{
    public int Id { get; init; }
    public string DealNumber { get; init; } = "";
    public string? InternalNumber { get; init; }
    public int StatusId { get; init; }
    public string StatusName { get; init; } = "";
    public int? LeadId { get; init; }
    public int ManagerId { get; init; }
    public string ManagerName { get; init; } = "";
    public int CompanyId { get; init; }
    public string CompanyName { get; init; } = "";
    public int? IssuerLegalEntityId { get; init; }
    public string? IssuerLegalEntityName { get; init; }
    public int? RequestTypeId { get; init; }
    public string? RequestTypeName { get; init; }
    public int? SourceId { get; init; }
    public string? SourceName { get; init; }

    public int? TourOperatorId { get; init; }
    public string? TourOperatorName { get; init; }
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
    public DateTimeOffset? LeadCreatedAt { get; set; }
    public string? CountryName { get; set; }
    public string? CityName { get; set; }
    public string? HotelName { get; set; }
    public string? RoomType { get; set; }
    public string? AccommodationType { get; set; }
    public string? BookingLink { get; set; }
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
    public IReadOnlyList<ClientShortDto> Customers { get; init; } = Array.Empty<ClientShortDto>();
    public IReadOnlyList<ClientShortDto> Tourists  { get; init; } = Array.Empty<ClientShortDto>();
    public IReadOnlyList<NewServiceDto> Services { get; init; } = Array.Empty<NewServiceDto>();
    public IReadOnlyList<NewPaymentDto> ClientPays { get; init; } = Array.Empty<NewPaymentDto>();
    public IReadOnlyList<NewPaymentDto> PartnerPays { get; init; } = Array.Empty<NewPaymentDto>();
}