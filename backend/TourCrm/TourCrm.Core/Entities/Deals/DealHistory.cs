namespace TourCrm.Core.Entities.Deals;

public class DealHistory
{
    public int Id { get; set; }

    public int DealId { get; set; }
    public Deal Deal { get; set; } = null!;

    public string Action { get; set; } = string.Empty; 
    public string? Note { get; set; }  

    public string? ActorUserId { get; set; }
    public string? ActorFullName { get; set; }

    public int CompanyId { get; set; }
    public Company Company { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
