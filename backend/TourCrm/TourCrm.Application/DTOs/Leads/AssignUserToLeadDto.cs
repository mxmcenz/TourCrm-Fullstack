namespace TourCrm.Application.DTOs.Leads;

public sealed record AssignUserToLeadDto
{
    public required int UserId { get; init; }
}