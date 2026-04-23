namespace TourCrm.Core.Entities.Tariffs;

public class TariffPermission
{
    public int Id { get; set; }
    public int TariffId { get; set; }
    public Tariff Tariff { get; set; } = null!;
    public string PermissionKey { get; set; } = string.Empty;
    public bool IsGranted { get; set; } = true;
}