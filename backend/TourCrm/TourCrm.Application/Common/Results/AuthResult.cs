using TourCrm.Application.DTOs;
using TourCrm.Application.DTOs.Auths;

namespace TourCrm.Application.Common.Results;

public class AuthResult
{
    public string Token { get; init; } = string.Empty;
    public UserStateDto User { get; init; } = null!;
}