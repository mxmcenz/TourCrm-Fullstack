namespace TourCrm.Application.DTOs.Auths;

public sealed record SetPasswordDto
{
    public int UserId { get; init; }
    public string Password { get; init; } = string.Empty;
    public string ConfirmPassword { get; init; } = string.Empty;
}