using TourCrm.Core.Enums;

namespace TourCrm.Application.DTOs.Clients;

public record ClientListItemDto
{
    public int Id { get; init; }
    public string FirstName { get; init; } = "";
    public string LastName { get; init; } = "";
    public string? PhoneE164 { get; init; }
    public string? Email { get; init; }
    public bool IsTourist { get; init; }
    public bool IsDeleted { get; init; }
    public ClientType ClientType { get; init; }

    public PassportDto? Passport { get; init; }
    public IdentityDocumentDto? IdentityDocument { get; init; }
    public BirthCertificateDto? BirthCertificate { get; init; }
    public List<InsurancePolicyDto> Insurances { get; init; } = new();
    public List<VisaRecordDto> Visas { get; init; } = new();
}