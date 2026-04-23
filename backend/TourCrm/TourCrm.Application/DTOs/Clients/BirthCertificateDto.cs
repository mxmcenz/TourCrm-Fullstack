namespace TourCrm.Application.DTOs.Clients;

public sealed record BirthCertificateDto(string SerialNumber, string IssuedBy, DateOnly? IssueDate);