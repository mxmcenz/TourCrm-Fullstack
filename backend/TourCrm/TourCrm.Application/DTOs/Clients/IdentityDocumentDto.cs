namespace TourCrm.Application.DTOs.Clients;

public sealed record IdentityDocumentDto(
    int? CitizenshipCountryId,
    int? ResidenceCountryId,
    int? ResidenceCityId,
    string? BirthPlace,
    string SerialNumber,
    string IssuedBy,
    DateOnly? IssueDate,
    string DocumentNumber,
    string? PersonalNumber,
    string? RegistrationAddress,
    string? ResidentialAddress,
    string? MotherFullName,
    string? FatherFullName,
    string? ContactInfo);