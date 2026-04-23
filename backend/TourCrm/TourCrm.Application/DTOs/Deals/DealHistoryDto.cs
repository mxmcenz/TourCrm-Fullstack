namespace TourCrm.Application.DTOs.Deals;

public record DealHistoryDto
{
    public int Id { get; init; }
    public string Action { get; init; } = string.Empty;
    public string? Note { get; init; }
    public string? ActorUserId { get; init; }
    public string? ActorFullName { get; init; }
    public DateTime CreatedAt { get; init; }
}