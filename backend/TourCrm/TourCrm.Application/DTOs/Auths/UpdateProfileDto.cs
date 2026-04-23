namespace TourCrm.Application.DTOs.Auths;

public sealed record UpdateProfileDto
{
    public string FullName { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
    public string Passport { get; init; } = string.Empty;
    public string TravelPreferences { get; init; } = string.Empty;
}