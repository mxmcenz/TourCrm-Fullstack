using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourCrm.Application.DTOs;
using TourCrm.Application.DTOs.Auths;
using TourCrm.Application.Interfaces;

namespace TourCrm.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService, IWebHostEnvironment env) : ControllerBase
{
    [HttpPost("start-registration")]
    public async Task<IActionResult> StartRegistration([FromBody] EmailPhoneNameDto nameDto)
    {
        var result = await authService.StartRegistrationAsync(nameDto);
        if (!result.Success) return BadRequest(new { message = result.Message });
        return Ok(new { message = result.Message, data = result.Data });
    }

    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailByCodeDto dto)
    {
        var result = await authService.VerifyEmailAsync(dto);
        if (!result.Success) return BadRequest(new { message = result.Message });
        return Ok(new { message = result.Message, data = result.Data });
    }

    [HttpPost("resend-code")]
    public async Task<IActionResult> ResendCode([FromBody] EmailDto dto)
    {
        var result = await authService.ResendConfirmationCodeAsync(dto);
        if (!result.Success) return BadRequest(new { message = result.Message });
        return Ok(new { message = result.Message, data = result.Data });
    }

    [HttpPost("resend-reset-code")]
    public async Task<IActionResult> ResendResetCode([FromBody] EmailDto dto)
    {
        var result = await authService.ResendResetCodeAsync(dto);
        if (!result.Success) return BadRequest(new { message = result.Message });
        return Ok(new { message = result.Message, data = result.Data });
    }

    [HttpPost("set-password")]
    public async Task<IActionResult> SetPassword([FromBody] SetPasswordDto dto)
    {
        var result = await authService.SetPasswordAsync(dto);
        if (!result.Success) return BadRequest(new { message = result.Message });

        SetAuthCookies(result.Data!);
        return Ok(new { message = result.Message, data = result.Data!.User });
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] EmailDto dto)
    {
        var result = await authService.ForgotPasswordAsync(dto);
        if (!result.Success) return BadRequest(new { message = result.Message });
        return Ok(new { message = result.Message, data = result.Data });
    }

    [HttpPost("verify-reset-code")]
    public async Task<IActionResult> VerifyResetCode([FromBody] VerifyResetCodeDto dto)
    {
        var result = await authService.VerifyResetCodeAsync(dto);
        if (!result.Success) return BadRequest(new { message = result.Message });
        return Ok(new { message = result.Message, data = result.Data });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] SetPasswordDto dto)
    {
        var result = await authService.ResetPasswordAsync(dto);
        if (!result.Success) return BadRequest(new { message = result.Message });

        SetAuthCookies(result.Data!);
        return Ok(new { message = result.Message, data = result.Data!.User });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var result = await authService.LoginAsync(dto);
        if (!result.Success) return BadRequest(new { message = result.Message });

        SetAuthCookies(result.Data!);
        return Ok(new { message = result.Message, data = result.Data!.User });
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var user = await authService.GetCurrentUserAsync(User);
        if (user == null) return Unauthorized(new { message = "Пользователь не найден" });

        var roles = User.FindAll(ClaimTypes.Role)
            .Select(c => c.Value)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        var permissions = User.FindAll("permissions")
            .Select(c => c.Value)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        int? companyId = int.TryParse(User.FindFirst("CompanyId")?.Value, out var cid) ? cid : null;
        var isSuperAdmin = roles.Any(r => string.Equals(r, "SuperAdmin", StringComparison.OrdinalIgnoreCase));

        return Ok(new
        {
            user.Id,
            user.Email,
            user.PhoneNumber,
            user.FirstName,
            user.LastName,
            user.MiddleName,
            user.IsEmailConfirmed,
            companyId,
            roles,
            permissions,
            isSuperAdmin
        });
    }

    [Authorize]
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        var del = BuildCookieOptions(expireNow: true);
        Response.Cookies.Delete("jwt", del);
        Response.Cookies.Delete("refresh-token", del);
        return Ok(new { message = "Выход выполнен" });
    }

    [Authorize]
    [HttpPut("update-profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
    {
        var result = await authService.UpdateProfileAsync(User, dto);
        if (!result.Success)
            return BadRequest(new { message = result.Message });

        return Ok(new { message = result.Message, data = result.Data });
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        var refreshToken = Request.Cookies["refresh-token"];
        if (string.IsNullOrWhiteSpace(refreshToken))
            return Unauthorized("Refresh токен отсутствует");

        var result = await authService.RefreshTokenAsync(refreshToken);
        if (!result.Success) return Unauthorized(new { message = result.Message });

        SetAuthCookies(result.Data!);
        return Ok(new { message = result.Message });
    }

    private void SetAuthCookies(TokenPairDto tokens)
    {
        var jwtOpts = BuildCookieOptions(minutes: tokens.AccessTokenExpiresIn);
        var rtOpts = BuildCookieOptions(days: tokens.RefreshTokenExpiresIn);

        Response.Cookies.Append("jwt", tokens.AccessToken, jwtOpts);
        Response.Cookies.Append("refresh-token", tokens.RefreshToken, rtOpts);
    }

    private CookieOptions BuildCookieOptions(int? minutes = null, int? days = null, bool expireNow = false)
    {
        var proto = Request.Headers["X-Forwarded-Proto"].FirstOrDefault();
        var isHttps = string.Equals(proto, "https", StringComparison.OrdinalIgnoreCase) || Request.IsHttps;

        var expires = expireNow
            ? DateTimeOffset.UtcNow.AddYears(-1)
            : minutes is not null
                ? DateTimeOffset.UtcNow.AddMinutes(minutes.Value)
                : days is not null
                    ? DateTimeOffset.UtcNow.AddDays(days.Value)
                    : (DateTimeOffset?)null;

        return new CookieOptions
        {
            HttpOnly = true,
            Secure = isHttps,
            SameSite = isHttps ? SameSiteMode.None : SameSiteMode.Lax,
            Path = "/",
            Expires = expires
        };
    }
}