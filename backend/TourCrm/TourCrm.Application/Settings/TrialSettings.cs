namespace TourCrm.Application.Settings;

public class TrialSettings
{
    public string TariffName { get; set; } = "Trial";
    public int Days { get; set; } = 14;
    public int MinEmployees { get; set; } = 1;
    public int MaxEmployees { get; set; } = 100;
}