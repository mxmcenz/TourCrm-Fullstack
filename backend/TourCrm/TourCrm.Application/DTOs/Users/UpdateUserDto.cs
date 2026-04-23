namespace TourCrm.Application.DTOs;

public class UpdateUserDto
{
    public string? LastName { get; init; }
    public string? FirstName { get; init; }
    public string? MiddleName { get; init; }
    public string? PhoneNumber { get; init; }
    public bool? IsEmailConfirmed { get; init; }
}