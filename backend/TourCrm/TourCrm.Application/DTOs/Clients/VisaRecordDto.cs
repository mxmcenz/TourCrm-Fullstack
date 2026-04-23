namespace TourCrm.Application.DTOs.Clients;

public sealed record VisaRecordDto(
    DateOnly? IssueDate,
    DateOnly? ExpireDate,
    int? CountryId,
    bool IsSchengen,
    string? Note);