namespace TourCrm.Application.DTOs;

public class CreateUserDto
{
    public string Email { get; init; } = string.Empty;
    public string? LastName { get; init; }
    public string? FirstName { get; init; }
    public string? MiddleName { get; init; }
    public string? PhoneNumber { get; init; }
}