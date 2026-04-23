namespace TourCrm.Application.DTOs.Clients;

public sealed record InsurancePolicyDto(DateOnly? IssueDate, DateOnly? ExpireDate, int? CountryId, string? Note);