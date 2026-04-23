using TourCrm.Core.Enums;

namespace TourCrm.Application.DTOs.Clients;

public sealed record UpdateClientDto
{
    public ClientType ClientType { get; init; }
    public int? ManagerId { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string? MiddleName { get; init; }
    public string? FirstNameGenitive { get; init; }
    public string? LastNameGenitive { get; init; }
    public string? MiddleNameGenitive { get; init; }
    public DateOnly? BirthDay { get; init; }
    public Gender Gender { get; init; }
    public string? PhoneE164 { get; init; }
    public string? Email { get; init; }
    public bool IsSubscribedToMailing { get; init; }
    public bool IsEmailNotificationEnabled { get; init; }
    public string? ReferredBy { get; init; }
    public string? Note { get; init; }
    public decimal DiscountPercent { get; init; }
    public bool IsTourist { get; init; }
    
    public PassportDto? Passport { get; init; }
    public IdentityDocumentDto? IdentityDocument { get; init; }
    public BirthCertificateDto? BirthCertificate { get; init; }
    public List<InsurancePolicyDto>? Insurances { get; init; }
    public List<VisaRecordDto>? Visas { get; init; }
}
