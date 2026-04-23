namespace TourCrm.Core.Entities.Dictionaries;

public class DealStatus
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public bool IsFinal { get; set; }

    public int CompanyId { get; set; }
    public Company Company { get; set; } = null!;
}