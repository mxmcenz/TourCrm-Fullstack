namespace TourCrm.Core.Entities.Dictionaries;

public class Hotel
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int CityId { get; set; }
    public int? Stars { get; set; }
    public int CompanyId { get; set; }
    public Company Company { get; set; } = null!;
}