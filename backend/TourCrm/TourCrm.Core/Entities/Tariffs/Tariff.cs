namespace TourCrm.Core.Entities.Tariffs;

public class Tariff
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int MinEmployees { get; set; }
    public int MaxEmployees { get; set; }
    public decimal MonthlyPrice { get; set; }
    public decimal? HalfYearPrice { get; set; }
    public decimal? YearlyPrice { get; set; }
    public bool IsPublic { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<TariffPermission> Permissions { get; set; } = new List<TariffPermission>();
    public ICollection<Company> Companies { get; set; } = new List<Company>();
}