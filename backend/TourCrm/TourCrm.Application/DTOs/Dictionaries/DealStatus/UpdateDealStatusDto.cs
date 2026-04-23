namespace TourCrm.Application.DTOs.Dictionaries.DealStatus;

public sealed class UpdateDealStatusDto
{
    public string Name { get; init; } = "";
    public bool IsFinal { get; init; }
}