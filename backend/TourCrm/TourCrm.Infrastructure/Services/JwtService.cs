using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TourCrm.Application.DTOs;
using TourCrm.Application.DTOs.Auths;
using TourCrm.Application.Interfaces;
using TourCrm.Application.Settings;

namespace TourCrm.Infrastructure.Services;

public class JwtService(IOptions<JwtSettings> options) : IJwtService
{
    private readonly JwtSettings _settings = options.Value;

    public string GenerateAccessToken(JwtPayloadDto dto)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, dto.UserId.ToString()),
            new(ClaimTypes.Email, dto.Email),
            new("CompanyId", dto.CompanyId.ToString())
        };
        claims.AddRange(dto.Roles.Distinct().Select(r => new Claim(ClaimTypes.Role, r)));
        claims.AddRange(dto.Permissions.Distinct().Select(p => new Claim("permissions", p)));

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_settings.AccessTokenExpirationMinutes),
            signingCredentials: credentials
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }
}