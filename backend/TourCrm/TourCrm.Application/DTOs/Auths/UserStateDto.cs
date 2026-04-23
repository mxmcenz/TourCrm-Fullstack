namespace TourCrm.Application.DTOs.Auths;

public sealed record UserStateDto
{
    public int UserId { get; init; }
    public string Email { get; init; } = string.Empty;
    public bool IsEmailConfirmed { get; init; }
    public string? FirstName { get; init; }
}