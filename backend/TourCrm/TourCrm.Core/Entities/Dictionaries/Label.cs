namespace TourCrm.Core.Entities.Dictionaries;

public class Label
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public bool IsActive { get; set; } = true;

    public int CompanyId { get; set; }
    public Company Company { get; set; } = null!;
}