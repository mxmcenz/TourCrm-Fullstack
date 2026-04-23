namespace TourCrm.Application.DTOs.Dictionaries.DealStatus;

public sealed class CreateDealStatusDto
{
    public string Name { get; init; } = "";
    public bool IsFinal { get; init; }
}