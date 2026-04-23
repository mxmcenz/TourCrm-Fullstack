namespace TourCrm.Core.Entities.Deals;

public class DealServiceItem
{
    public int Id { get; set; }
    public int DealId { get; set; }
    public Deal Deal { get; set; } = null!;

    public string Name { get; set; } = null!;
    public string? Note { get; set; }

    public int? CompanyId { get; set; }
}