namespace TourCrm.Application.DTOs;

public class UserDto
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public bool IsEmailConfirmed { get; set; }
    public string FullName { get; set; } = string.Empty;
}