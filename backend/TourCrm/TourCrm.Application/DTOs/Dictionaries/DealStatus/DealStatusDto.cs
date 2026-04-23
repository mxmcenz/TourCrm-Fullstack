namespace TourCrm.Application.DTOs.Dictionaries.DealStatus;

public sealed class DealStatusDto
{
    public int Id { get; init; }
    public string Name { get; init; } = "";
    public bool IsFinal { get; init; }
}