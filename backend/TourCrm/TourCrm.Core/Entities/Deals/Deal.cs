using TourCrm.Core.Entities.Dictionaries;
using TourCrm.Core.Entities.Leads;

namespace TourCrm.Core.Entities.Deals;

public class Deal
{
    public int Id { get; set; }
    public string DealNumber { get; set; } = string.Empty;
    public string? InternalNumber { get; set; }
    public int StatusId { get; set; }
    public DealStatus Status { get; set; } = null!;
    public int? LeadId { get; set; }
    public Lead? Lead { get; set; }
    public int ManagerId { get; set; }
    public Employee Manager { get; set; } = null!;
    public int CompanyId { get; set; }
    public Company Company { get; set; } = null!;
    public int? IssuerLegalEntityId { get; set; }
    public LegalEntity? IssuerLegalEntity { get; set; }
    public int? RequestTypeId { get; set; }
    public LeadRequestType? RequestType { get; set; }
    public int? SourceId { get; set; }
    public LeadSource? Source { get; set; }
    public int? TourOperatorId { get; set; }
    public TourOperator? TourOperator { get; set; }
    public int? VisaTypeId { get; set; }
    public string TourName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? BookingNumbers { get; set; }
    public string? Note { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public DateOnly? ClientPaymentDeadline { get; set; }
    public DateOnly? PartnerPaymentDeadline { get; set; }
    public DateOnly? DocsPackageDate { get; set; }
    public bool AddStampAndSign { get; set; }
    public bool IncludeCostCalc { get; set; }
    public bool IncludeTourCalc { get; set; }
    public string CreatedByUserId { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
    public ICollection<Client.Client> Customers { get; set; } = new List<Client.Client>();
    public ICollection<Client.Client> Tourists  { get; set; } = new List<Client.Client>();
    public ICollection<DealServiceItem> Services { get; set; } = new List<DealServiceItem>();
    public ICollection<DealClientPayment> ClientPayments { get; set; } = new List<DealClientPayment>();
    public ICollection<DealPartnerPayment> PartnerPayments { get; set; } = new List<DealPartnerPayment>();
}