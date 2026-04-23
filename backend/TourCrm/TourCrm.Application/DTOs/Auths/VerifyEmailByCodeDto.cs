namespace TourCrm.Application.DTOs.Auths;

public sealed record VerifyEmailByCodeDto
{
    public int UserId { get; init; }
    public string Code { get; init; } = string.Empty;
}