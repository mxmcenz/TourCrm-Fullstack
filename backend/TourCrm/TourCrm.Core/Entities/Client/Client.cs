using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using TourCrm.Core.Common;
using TourCrm.Core.Enums;

namespace TourCrm.Core.Entities.Client;

[Index(nameof(CompanyId), nameof(Email))]
[Index(nameof(CompanyId), nameof(PhoneE164))]
public class Client : BaseEntity
{
    public ClientType ClientType { get; set; }

    public int? ManagerId { get; set; }

    [MaxLength(100)] public string FirstName { get; set; } = string.Empty;
    [MaxLength(100)] public string LastName { get; set; } = string.Empty;
    [MaxLength(100)] public string MiddleName { get; set; } = string.Empty;

    [MaxLength(100)] public string? FirstNameGenitive { get; set; }
    [MaxLength(100)] public string? LastNameGenitive { get; set; }
    [MaxLength(100)] public string? MiddleNameGenitive { get; set; }

    public DateOnly? BirthDay { get; set; }
    public Gender Gender { get; set; }

    [MaxLength(20)] public string? PhoneE164 { get; set; }
    [MaxLength(254)] public string? Email { get; set; }

    public bool IsSubscribedToMailing { get; set; }
    public bool IsEmailNotificationEnabled { get; set; }

    [MaxLength(150)] public string? ReferredBy { get; set; }
    [MaxLength(2000)] public string? Note { get; set; }

    [Precision(5, 2)] public decimal DiscountPercent { get; set; }

    public bool IsTourist { get; set; }

    public Passport? Passport { get; set; }
    public IdentityDocument? IdentityDocument { get; set; }
    public BirthCertificate? BirthCertificate { get; set; }
    public ICollection<InsurancePolicy> Insurances { get; set; } = new List<InsurancePolicy>();
    public ICollection<VisaRecord> Visas { get; set; } = new List<VisaRecord>();
}