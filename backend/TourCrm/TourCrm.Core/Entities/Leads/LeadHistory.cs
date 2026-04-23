namespace TourCrm.Core.Entities.Leads;

public class LeadHistory
{
    public int Id { get; set; }
    public int LeadId { get; set; }
    public Lead Lead { get; set; } = default!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string Action { get; set; } = default!;
    public string ActorUserId { get; set; } = default!;
    public string? ActorFullName { get; set; }
    public string SnapshotJson { get; set; } = default!;

    public int CompanyId { get; set; }
    public Company Company { get; set; } = null!;
}