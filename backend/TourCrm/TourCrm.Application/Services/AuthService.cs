using System.Security.Claims;
using Microsoft.Extensions.Options;
using TourCrm.Application.Common.Results;
using TourCrm.Application.DTOs;
using TourCrm.Application.DTOs.Auths;
using TourCrm.Application.Interfaces;
using TourCrm.Application.Settings;
using TourCrm.Core.Entities;
using TourCrm.Core.Interfaces;

namespace TourCrm.Application.Services;

public class AuthService(
    IUnitOfWork unitOfWork,
    IPasswordHasher hasher,
    IJwtService jwt,
    IOptions<JwtSettings> jwtSettings,
    IEmailService emailService,
    ICompanyService companyService) : IAuthService
{
    public async Task<ServiceResult<object>> StartRegistrationAsync(EmailPhoneNameDto dto)
    {
        var existing = await unitOfWork.Users.GetByEmailAsync(dto.Email);
        if (existing != null)
            return ServiceResult<object>.Fail("Email уже зарегистрирован");

        var code = emailService.GenerateConfirmationCode();
        var user = new User
        {
            Email = dto.Email.Trim(),
            PhoneNumber = dto.PhoneNumber.Trim(),
            FirstName = dto.FirstName.Trim(),
            ConfirmationCode = code,
            ConfirmationCodeGeneratedAt = DateTime.UtcNow
        };

        await unitOfWork.Users.AddAsync(user);
        await unitOfWork.SaveChangesAsync();
        
        var html = EmailTemplates.VerificationHtml("TourCRM", code);
        await emailService.SendCodeAsync(dto.Email, "Код подтверждения • TourCRM", $"Код подтверждения для TourCRM: {code}", html);
        
        return ServiceResult<object>.Ok(new
        {
            userId = user.Id,
            email = user.Email,
            firstName = user.FirstName,
            code
        }, "Регистрация начата");
    }

    public async Task<ServiceResult<object>> VerifyEmailAsync(VerifyEmailByCodeDto dto)
    {
        var user = await unitOfWork.Users.GetByIdAsync(dto.UserId);
        if (user == null)
            return ServiceResult<object>.Fail("Пользователь не найден");

        if (user.IsEmailConfirmed)
            return ServiceResult<object>.Ok("Email уже подтверждён");

        if (user.ConfirmationCodeGeneratedAt is not { } generatedAt ||
            DateTime.UtcNow - generatedAt > TimeSpan.FromMinutes(10))
            return ServiceResult<object>.Fail("Срок действия кода истёк. Запросите новый.");

        if (user.ConfirmationCode != dto.Code)
            return ServiceResult<object>.Fail("Неверный код");

        user.IsEmailConfirmed = true;
        user.ConfirmationCode = string.Empty;
        user.ConfirmationCodeGeneratedAt = null;

        await unitOfWork.SaveChangesAsync();

        return ServiceResult<object>.Ok(new
        {
            userId = user.Id,
            email = user.Email,
            isEmailConfirmed = user.IsEmailConfirmed
        }, "Email подтверждён");
    }

    public async Task<ServiceResult<TokenPairDto>> SetPasswordAsync(SetPasswordDto dto)
    {
        if (dto.Password != dto.ConfirmPassword)
            return ServiceResult<TokenPairDto>.Fail("Пароли не совпадают");

        var user = await unitOfWork.Users.GetByIdAsync(dto.UserId);
        if (user == null)
            return ServiceResult<TokenPairDto>.Fail("Пользователь не найден");

        if (!user.IsEmailConfirmed)
            return ServiceResult<TokenPairDto>.Fail("Email не подтверждён");

        if (!string.IsNullOrEmpty(user.PasswordHash))
            return ServiceResult<TokenPairDto>.Fail("Пароль уже установлен");

        user.PasswordHash = hasher.Hash(dto.Password);
        await unitOfWork.SaveChangesAsync();

        await companyService.CreateIfMissingAsync(user.Id.ToString(), BuildCompanyName(user));

        var tokens = await GenerateTokensAsync(user);
        return ServiceResult<TokenPairDto>.Ok(tokens, "Пароль установлен. Вход выполнен");
    }

    public async Task<ServiceResult<object>> ForgotPasswordAsync(EmailDto dto)
    {
        var user = await unitOfWork.Users.GetByEmailAsync(dto.Email);
        if (user == null)
            return ServiceResult<object>.Fail("Пользователь с таким Email не найден");

        user.ConfirmationCode = emailService.GenerateConfirmationCode();
        user.ConfirmationCodeGeneratedAt = DateTime.UtcNow;

        await unitOfWork.SaveChangesAsync();
        var htmlReset = EmailTemplates.VerificationHtml("TourCRM", user.ConfirmationCode);
        var textReset = $"Код для сброса пароля в TourCRM: {user.ConfirmationCode}.";
        await emailService.SendCodeAsync(dto.Email, "Код для сброса пароля • TourCRM", textReset, htmlReset);
        
        return ServiceResult<object>.Ok(new
        {
            userId = user.Id,
            email = user.Email,
            isEmailConfirmed = user.IsEmailConfirmed,
            code = user.ConfirmationCode
        }, "Код для сброса пароля отправлен на почту");
    }

    public async Task<ServiceResult<object>> VerifyResetCodeAsync(VerifyResetCodeDto dto)
    {
        var user = await unitOfWork.Users.GetByIdAsync(dto.UserId);
        if (user == null)
            return ServiceResult<object>.Fail("Пользователь не найден");

        if (user.ConfirmationCodeGeneratedAt is not { } generatedAt ||
            DateTime.UtcNow - generatedAt > TimeSpan.FromMinutes(10))
            return ServiceResult<object>.Fail("Срок действия кода истёк. Запросите новый.");

        if (user.ConfirmationCode != dto.Code)
            return ServiceResult<object>.Fail("Неверный код");

        return ServiceResult<object>.Ok(new
        {
            userId = user.Id,
            email = user.Email,
            isEmailConfirmed = user.IsEmailConfirmed
        }, "Код подтверждён");
    }

    public async Task<ServiceResult<TokenPairDto>> ResetPasswordAsync(SetPasswordDto dto)
    {
        if (dto.Password != dto.ConfirmPassword)
            return ServiceResult<TokenPairDto>.Fail("Пароли не совпадают");

        var user = await unitOfWork.Users.GetByIdAsync(dto.UserId);
        if (user == null)
            return ServiceResult<TokenPairDto>.Fail("Пользователь не найден");

        if (string.IsNullOrWhiteSpace(user.ConfirmationCode))
            return ServiceResult<TokenPairDto>.Fail("Сначала запросите код сброса");

        user.PasswordHash = hasher.Hash(dto.Password);
        user.ConfirmationCode = string.Empty;
        user.ConfirmationCodeGeneratedAt = null;

        await unitOfWork.SaveChangesAsync();

        var tokens = await GenerateTokensAsync(user);
        return ServiceResult<TokenPairDto>.Ok(tokens, "Пароль успешно обновлён. Вход выполнен");
    }

    public async Task<ServiceResult<TokenPairDto>> LoginAsync(LoginDto dto)
    {
        var user = await unitOfWork.Users.GetByEmailAsync(dto.Email);
        if (user == null || !hasher.Verify(user.PasswordHash!, dto.Password))
            return ServiceResult<TokenPairDto>.Fail("Неверный email или пароль");

        if (!user.IsEmailConfirmed)
            return ServiceResult<TokenPairDto>.Fail("Email не подтверждён");

        var tokens = await GenerateTokensAsync(user);
        return ServiceResult<TokenPairDto>.Ok(tokens, "Вход выполнен");
    }

    public async Task<User?> GetCurrentUserAsync(ClaimsPrincipal principal)
    {
        var userId = GetUserId(principal);
        return userId == null ? null : await unitOfWork.Users.GetByIdAsync(userId.Value);
    }

    public async Task<ServiceResult<object>> UpdateProfileAsync(ClaimsPrincipal principal, UpdateProfileDto dto)
    {
        var userId = GetUserId(principal);
        if (userId == null)
            return ServiceResult<object>.Fail("Ошибка авторизации");

        var user = await unitOfWork.Users.GetByIdAsync(userId.Value);
        if (user == null)
            return ServiceResult<object>.Fail("Пользователь не найден");


        user.PhoneNumber = dto.PhoneNumber;


        await unitOfWork.SaveChangesAsync();

        return ServiceResult<object>.Ok(new
        {
            userId = user.Id,
            email = user.Email,
            isEmailConfirmed = user.IsEmailConfirmed
        }, "Профиль обновлён");
    }

    public async Task<ServiceResult<object>> ResendConfirmationCodeAsync(EmailDto dto)
    {
        var user = await unitOfWork.Users.GetByEmailAsync(dto.Email);
        if (user == null)
            return ServiceResult<object>.Fail("Пользователь с таким Email не найден");

        if (user.IsEmailConfirmed)
            return ServiceResult<object>.Fail("Email уже подтверждён");

        user.ConfirmationCode = emailService.GenerateConfirmationCode();
        user.ConfirmationCodeGeneratedAt = DateTime.UtcNow;

        await unitOfWork.SaveChangesAsync();
        var htmlConfirm = EmailTemplates.VerificationHtml("TourCRM", user.ConfirmationCode);
        var textConfirm = $"Новый код подтверждения для TourCRM: {user.ConfirmationCode}.";
        await emailService.SendCodeAsync(dto.Email, "Код подтверждения • TourCRM", textConfirm, htmlConfirm);
        
        return ServiceResult<object>.Ok(new
        {
            userId = user.Id,
            email = user.Email,
            code = user.ConfirmationCode
        }, "Код подтверждения отправлен повторно");
    }

    public async Task<ServiceResult<object>> ResendResetCodeAsync(EmailDto dto)
    {
        var user = await unitOfWork.Users.GetByEmailAsync(dto.Email);
        if (user == null)
            return ServiceResult<object>.Fail("Пользователь с таким Email не найден");

        user.ConfirmationCode = emailService.GenerateConfirmationCode();
        user.ConfirmationCodeGeneratedAt = DateTime.UtcNow;

        await unitOfWork.SaveChangesAsync();
        var htmlResend = EmailTemplates.VerificationHtml("TourCRM", user.ConfirmationCode);
        var textResend = $"Новый код для сброса пароля в TourCRM: {user.ConfirmationCode}.";
        await emailService.SendCodeAsync(dto.Email, "Код для сброса пароля • TourCRM", textResend, htmlResend);
        return ServiceResult<object>.Ok(new
        {
            userId = user.Id,
            email = user.Email,
            code = user.ConfirmationCode
        }, "Код отправлен повторно");
    }

    public async Task<ServiceResult<TokenPairDto>> RefreshTokenAsync(string refreshToken)
    {
        var stored = await unitOfWork.RefreshTokens.GetByTokenAsync(refreshToken);
        if (stored == null || stored.IsRevoked || stored.ExpiresAt < DateTime.UtcNow)
            return ServiceResult<TokenPairDto>.Fail("Недействительный токен");

        stored.IsRevoked = true;

        var user = await unitOfWork.Users.GetByIdAsync(stored.UserId);
        if (user == null)
            return ServiceResult<TokenPairDto>.Fail("Пользователь не найден");

        var tokens = await GenerateTokensAsync(user);
        await unitOfWork.SaveChangesAsync();

        return ServiceResult<TokenPairDto>.Ok(tokens, "Токены обновлены");
    }

    private static string BuildCompanyName(User user)
    {
        var parts = new[] { user.LastName, user.FirstName, user.MiddleName }
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Select(s => s!.Trim())
            .ToArray();

        var fio = string.Join(" ", parts);
        if (!string.IsNullOrWhiteSpace(fio))
            return fio;

        var emailName = (user.Email ?? "").Split('@').FirstOrDefault();
        return string.IsNullOrWhiteSpace(emailName) ? "Моя компания" : emailName;
    }

    private async Task<TokenPairDto> GenerateTokensAsync(User user)
    {
        var roles = await unitOfWork.UserRoles.GetRolesByUserIdAsync(user.Id);
        var permissions = await unitOfWork.UserRoles.GetPermissionsByUserIdAsync(user.Id);

        var myCompany = await unitOfWork.Companies.GetByOwnerAsync(user.Id.ToString());
        int? companyId = myCompany?.Id;

        if (companyId is null)
        {
            var userRoleIds = (await unitOfWork.UserRoles.GetAllAsync())
                .Where(ur => ur.UserId == user.Id)
                .Select(ur => ur.RoleId)
                .ToHashSet();

            var role = (await unitOfWork.Roles.GetAllAsync())
                .FirstOrDefault(r =>
                    userRoleIds.Contains(r.Id) &&
                    !string.Equals(r.Name, "SuperAdmin", StringComparison.OrdinalIgnoreCase));

            companyId = role?.CompanyId;
        }

        if (companyId is null)
            throw new InvalidOperationException("Пользователь не привязан к компании.");

        var accessToken = jwt.GenerateAccessToken(new JwtPayloadDto
        {
            UserId = user.Id,
            Email = user.Email,
            Roles = roles,
            Permissions = permissions,
            CompanyId = companyId.Value
        });

        var refreshToken = jwt.GenerateRefreshToken();
        await unitOfWork.RefreshTokens.AddAsync(new RefreshToken
        {
            Token = refreshToken, UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(jwtSettings.Value.RefreshTokenExpirationDays), IsRevoked = false
        });
        await unitOfWork.SaveChangesAsync();

        return new TokenPairDto
        {
            AccessToken = accessToken,
            AccessTokenExpiresIn = jwtSettings.Value.AccessTokenExpirationMinutes,
            RefreshToken = refreshToken,
            RefreshTokenExpiresIn = jwtSettings.Value.RefreshTokenExpirationDays,
            User = new UserStateDto
            {
                UserId = user.Id, Email = user.Email, IsEmailConfirmed = user.IsEmailConfirmed,
                FirstName = user.FirstName
            }
        };
    }

    private static int? GetUserId(ClaimsPrincipal principal)
    {
        var claim = principal.FindFirst(ClaimTypes.NameIdentifier);
        return int.TryParse(claim?.Value, out var id) ? id : null;
    }
}