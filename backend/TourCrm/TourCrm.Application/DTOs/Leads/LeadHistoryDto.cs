namespace TourCrm.Application.DTOs.Leads;

public sealed record LeadHistoryDto
{
    public DateTime CreatedAt { get; init; }
    public string Action { get; init; } = string.Empty;
    public string ActorUserId { get; init; } = string.Empty;
    public string? ActorFullName { get; init; }
    public string SnapshotJson { get; init; } = string.Empty;
}