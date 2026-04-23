namespace TourCrm.Core.Entities;

public class Agency
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<Office> Offices { get; set; } = new List<Office>();
}