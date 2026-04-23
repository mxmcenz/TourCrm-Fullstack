namespace TourCrm.Application.DTOs.Clients;

public sealed record PassportDto(
    string FirstNameLatin,
    string LastNameLatin,
    string SerialNumber,
    DateOnly? IssueDate,
    DateOnly? ExpireDate,
    string IssuingAuthority);