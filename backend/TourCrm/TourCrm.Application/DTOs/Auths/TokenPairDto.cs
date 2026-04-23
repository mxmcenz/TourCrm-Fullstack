namespace TourCrm.Application.DTOs.Auths;

public sealed class TokenPairDto
{
    public string AccessToken { get; init; } = default!;
    public int AccessTokenExpiresIn { get; init; } 

    public string RefreshToken { get; init; } = default!;
    public int RefreshTokenExpiresIn { get; init; } 

    public UserStateDto User { get; init; } = default!;
}