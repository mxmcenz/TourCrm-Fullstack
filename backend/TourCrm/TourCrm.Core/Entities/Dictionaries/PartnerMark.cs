namespace TourCrm.Core.Entities.Dictionaries;

public class PartnerMark
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int CompanyId { get; set; }
    public Company Company { get; set; }
}