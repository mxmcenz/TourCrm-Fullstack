namespace TourCrm.Application.DTOs.Auths;

public sealed record LoginDto
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}