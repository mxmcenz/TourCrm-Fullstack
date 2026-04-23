namespace TourCrm.Core.Entities.Deals;

public class DealCustomer
{
    public int Id { get; set; }
    public int DealId { get; set; }
    public Deal Deal { get; set; } = null!;

    public string FullName { get; set; } = null!;
    public string? Phone { get; set; }
    public string? Email { get; set; }

    public int? CompanyId { get; set; }
}