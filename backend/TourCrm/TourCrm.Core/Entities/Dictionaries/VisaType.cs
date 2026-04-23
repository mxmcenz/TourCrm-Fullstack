namespace TourCrm.Core.Entities.Dictionaries;

public sealed class VisaType
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;

    public int CompanyId { get; set; }
    public Company Company { get; set; } = null!;
}