using TourCrm.Application.DTOs;
using TourCrm.Application.DTOs.Auths;

namespace TourCrm.Application.Interfaces;

public interface IJwtService
{
    string GenerateAccessToken(JwtPayloadDto dto);
    string GenerateRefreshToken();
}