using System.Security.Claims;
using TourCrm.Application.Common.Results;
using TourCrm.Application.DTOs;
using TourCrm.Application.DTOs.Auths;
using TourCrm.Core.Entities;

namespace TourCrm.Application.Interfaces;

public interface IAuthService
{
    Task<ServiceResult<object>> StartRegistrationAsync(EmailPhoneNameDto dto);
    Task<ServiceResult<object>> VerifyEmailAsync(VerifyEmailByCodeDto dto);
    Task<ServiceResult<TokenPairDto>> SetPasswordAsync(SetPasswordDto dto);
    Task<ServiceResult<object>> ForgotPasswordAsync(EmailDto dto);
    Task<ServiceResult<object>> VerifyResetCodeAsync(VerifyResetCodeDto dto);
    Task<ServiceResult<TokenPairDto>> ResetPasswordAsync(SetPasswordDto dto);
    Task<ServiceResult<TokenPairDto>> LoginAsync(LoginDto dto);
    Task<User?> GetCurrentUserAsync(ClaimsPrincipal user);
    Task<ServiceResult<object>> UpdateProfileAsync(ClaimsPrincipal user, UpdateProfileDto dto);
    Task<ServiceResult<object>> ResendConfirmationCodeAsync(EmailDto dto);
    Task<ServiceResult<object>> ResendResetCodeAsync(EmailDto dto);
    Task<ServiceResult<TokenPairDto>> RefreshTokenAsync(string refreshToken);
}