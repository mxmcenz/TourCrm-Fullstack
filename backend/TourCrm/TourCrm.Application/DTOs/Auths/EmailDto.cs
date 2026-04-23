namespace TourCrm.Application.DTOs.Auths;

public sealed record EmailDto
{
    public string Email { get; init; } = string.Empty;
}