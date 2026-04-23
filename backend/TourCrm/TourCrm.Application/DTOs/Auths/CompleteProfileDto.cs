namespace TourCrm.Application.DTOs.Auths;

public sealed record CompleteProfileDto
{
    public int UserId { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
    public string Passport { get; init; } = string.Empty;
    public string TravelPreferences { get; init; } = string.Empty;
}