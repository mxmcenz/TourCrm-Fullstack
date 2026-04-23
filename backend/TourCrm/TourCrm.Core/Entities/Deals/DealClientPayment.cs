namespace TourCrm.Core.Entities.Deals;

public class DealClientPayment
{
    public int Id { get; set; }
    public int DealId { get; set; }
    public Deal Deal { get; set; } = null!;

    public string Title { get; set; } = null!;
    public decimal? Amount { get; set; }

    public int? CompanyId { get; set; }
}