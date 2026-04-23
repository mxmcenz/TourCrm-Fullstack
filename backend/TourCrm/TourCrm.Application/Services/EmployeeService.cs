using Microsoft.Extensions.Logging;
using TourCrm.Application.Common.Results;
using TourCrm.Application.DTOs;
using TourCrm.Application.DTOs.Employees;
using TourCrm.Application.Interfaces;
using TourCrm.Application.Settings;
using TourCrm.Core.Entities;
using TourCrm.Core.Entities.Roles;
using TourCrm.Core.Interfaces;

namespace TourCrm.Application.Services;

public class EmployeeService(
    IUnitOfWork unitOfWork,
    IPasswordHasher hasher,
    IEmailService emailService,
    ILogger<EmployeeService> logger,
    ICompanyRepository companyRepo
) : IEmployeeService
{
    private const string SuperAdminRoleName = "SuperAdmin";

    private async Task<bool> IsSuperAdminAsync(string userId, CancellationToken ct)
    {
        if (!int.TryParse(userId, out var uid)) return false;
        var userRoles = (await unitOfWork.UserRoles.GetAllAsync(ct)).ToList();
        if (userRoles.Count == 0) return false;
        var roleIds = userRoles.Where(ur => ur.UserId == uid).Select(ur => ur.RoleId).ToHashSet();
        if (roleIds.Count == 0) return false;
        var roles = (await unitOfWork.Roles.GetAllAsync(ct)).ToList();
        return roles.Any(r =>
            roleIds.Contains(r.Id) && r.Name.Equals(SuperAdminRoleName, StringComparison.OrdinalIgnoreCase));
    }

    private async Task<HashSet<int>> GetMyLegalEntityIdsAsync(string userId, CancellationToken ct)
    {
        var companyByOwner = await companyRepo.GetByOwnerAsync(userId, ct);
        if (companyByOwner != null)
            return companyByOwner.LegalEntities.Select(le => le.Id).ToHashSet();

        if (!int.TryParse(userId, out var uid))
            throw new InvalidOperationException("Некорректный идентификатор пользователя.");

        var employee = await unitOfWork.Employees.GetByIdAsync(uid);
        if (employee == null)
            throw new UnauthorizedAccessException("Пользователь не найден.");
        if (employee.LegalEntityId <= 0)
            throw new InvalidOperationException("Пользователь не привязан к юрлицу.");

        var companyByLe = await companyRepo.GetByLegalEntityIdAsync(employee.LegalEntityId, ct)
                          ?? throw new InvalidOperationException("Компания для указанного юрлица не найдена.");

        return companyByLe.LegalEntities.Select(le => le.Id).ToHashSet();
    }

    public async Task<IEnumerable<EmployeeDto>> GetAllAsync(string userId, CancellationToken ct = default)
    {
        var isSa = await IsSuperAdminAsync(userId, ct);
        var employees = (await unitOfWork.Employees.GetAllAsync()).ToList();
        if (isSa) return employees.Select(MapToDto);
        var leIds = await GetMyLegalEntityIdsAsync(userId, ct);
        return employees.Where(e => leIds.Contains(e.LegalEntityId)).Select(MapToDto);
    }

    public async Task<EmployeeDto?> GetByIdAsync(int id, string userId, CancellationToken ct = default)
    {
        var isSa = await IsSuperAdminAsync(userId, ct);
        var e = await unitOfWork.Employees.GetByIdAsync(id);
        if (e == null) return null;
        if (!isSa)
        {
            var leIds = await GetMyLegalEntityIdsAsync(userId, ct);
            if (!leIds.Contains(e.LegalEntityId)) return null;
        }

        return MapToDto(e);
    }

    public async Task<EmployeeDto> CreateAsync(EmployeeCreateDto dto, string userId, CancellationToken ct = default)
    {
        var isSa = await IsSuperAdminAsync(userId, ct);

        if (!isSa)
        {
            var leIds = await GetMyLegalEntityIdsAsync(userId, ct);
            if (dto.LegalEntityId <= 0 || !leIds.Contains(dto.LegalEntityId))
                throw new UnauthorizedAccessException("Нет доступа к указанному юрлицу.");
        }
        else
        {
            if (dto.LegalEntityId <= 0)
                throw new InvalidOperationException("Не указан LegalEntityId.");
        }

        var existingByEmail = await unitOfWork.Users.GetByEmailAsync(dto.Email, ct);
        if (existingByEmail != null)
            throw new InvalidOperationException($"Пользователь с почтой '{dto.Email}' уже зарегистрирован");

        if (!string.IsNullOrWhiteSpace(dto.Phone))
        {
            var normalizedPhone = dto.Phone.Trim();
            var existingByPhone = await unitOfWork.Users.GetByPhoneAsync(normalizedPhone);
            if (existingByPhone != null)
                throw new InvalidOperationException($"Пользователь с номером '{normalizedPhone}' уже зарегистрирован");
        }

        var employee = new Employee
        {
            Email = dto.Email,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            MiddleName = dto.MiddleName,
            PhoneNumber = string.IsNullOrWhiteSpace(dto.Phone) ? string.Empty : dto.Phone.Trim(),
            PasswordHash = hasher.Hash(dto.Password),
            IsEmailConfirmed = true,
            OfficeId = dto.OfficeId,
            LegalEntityId = dto.LegalEntityId,
            LeadLimit = dto.LeadLimit,
            IsDeleted = dto.IsDeleted,
            Position = dto.Position,
            PositionGenitive = dto.PositionGenitive,
            PowerOfAttorneyNumber = dto.PowerOfAttorneyNumber,
            LastNameGenitive = dto.LastNameGenitive,
            FirstNameGenitive = dto.FirstNameGenitive,
            MiddleNameGenitive = dto.MiddleNameGenitive,
            MobilePhone = dto.MobilePhone,
            AdditionalPhone = dto.AdditionalPhone,
            BirthDate = dto.BirthDate,
            TimeZone = dto.TimeZone,
            ContactInfo = dto.ContactInfo,
            HireDate = dto.HireDate,
            SalaryAmount = dto.SalaryAmount,
            WorkConditions = dto.WorkConditions,
            Note = dto.Note
        };

        foreach (var roleId in dto.RoleIds)
        {
            var role = await unitOfWork.Roles.GetByIdAsync(roleId, ct);
            if (role is { CompanyId: not null })
            {
                employee.UserRoles.Add(new UserRole { User = employee, RoleId = roleId, Role = role });
            }
        }

        await unitOfWork.Employees.AddAsync(employee, ct);
        await unitOfWork.SaveChangesAsync(ct);

        try
        {
            var subject = "Ваши данные для входа в TourCRM";
            var textBody =
                $@"Здравствуйте, {employee.FirstName} {employee.LastName}!
Вам создана учётная запись в TourCRM.
Логин (email): {dto.Email}
Пароль: {dto.Password}
Ссылка для входа: https://app.tourcrm.example";

            var htmlBody = EmailTemplates.CredentialsHtml(
                productName: "TourCRM",
                firstName: employee.FirstName,
                lastName: employee.LastName,
                loginEmail: dto.Email,
                password: dto.Password,
                signInUrl: "http://46.101.125.0/"
            );

            await emailService.SendCodeAsync(dto.Email, subject, textBody, htmlBody);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Не удалось отправить письмо с данными для входа на {Email}", dto.Email);
        }


        var createdEmployee = await unitOfWork.Employees.GetByIdAsync(employee.Id);
        return MapToDto(createdEmployee!);
    }

    public async Task<bool> UpdateAsync(int id, EmployeeUpdateDto dto, string userId, CancellationToken ct = default)
    {
        var isSa = await IsSuperAdminAsync(userId, ct);
        var employee = await unitOfWork.Employees.GetByIdAsync(id);
        if (employee == null || employee.IsDeleted) return false;

        if (!isSa)
        {
            var leIds = await GetMyLegalEntityIdsAsync(userId, ct);
            if (!leIds.Contains(employee.LegalEntityId)) return false;
            if (dto.LegalEntityId <= 0 || !leIds.Contains(dto.LegalEntityId))
                throw new UnauthorizedAccessException("Нет доступа к указанному юрлицу.");
        }
        else
        {
            if (dto.LegalEntityId <= 0)
                throw new InvalidOperationException("Не указан LegalEntityId.");
        }

        employee.OfficeId = dto.OfficeId;
        employee.LegalEntityId = dto.LegalEntityId;
        employee.Email = dto.Email;
        employee.FirstName = dto.FirstName;
        employee.LastName = dto.LastName;
        employee.MiddleName = dto.MiddleName;
        employee.PhoneNumber = string.IsNullOrWhiteSpace(dto.Phone) ? string.Empty : dto.Phone.Trim();
        employee.LeadLimit = dto.LeadLimit;
        employee.IsDeleted = dto.IsDeleted;
        employee.Position = dto.Position;
        employee.PositionGenitive = dto.PositionGenitive;
        employee.PowerOfAttorneyNumber = dto.PowerOfAttorneyNumber;
        employee.LastNameGenitive = dto.LastNameGenitive;
        employee.FirstNameGenitive = dto.FirstNameGenitive;
        employee.MiddleNameGenitive = dto.MiddleNameGenitive;
        employee.MobilePhone = dto.MobilePhone;
        employee.AdditionalPhone = dto.AdditionalPhone;
        employee.BirthDate = dto.BirthDate;
        employee.TimeZone = dto.TimeZone;
        employee.ContactInfo = dto.ContactInfo;
        employee.HireDate = dto.HireDate;
        employee.SalaryAmount = dto.SalaryAmount;
        employee.WorkConditions = dto.WorkConditions;
        employee.Note = dto.Note;

        if (!string.IsNullOrWhiteSpace(dto.Password))
            employee.PasswordHash = hasher.Hash(dto.Password);

        await RecreateEmployeeRoles(employee, dto.RoleIds);

        unitOfWork.Employees.Update(employee);
        await unitOfWork.SaveChangesAsync(ct);
        return true;
    }

    private async Task RecreateEmployeeRoles(Employee employee, IEnumerable<int>? newRoleIds)
    {
        employee.UserRoles.Clear();
        if (newRoleIds == null) return;

        foreach (var roleId in newRoleIds)
        {
            var role = await unitOfWork.Roles.GetByIdAsync(roleId);
            if (role is { CompanyId: not null })
                employee.UserRoles.Add(new UserRole { User = employee, RoleId = roleId });
        }
    }

    public async Task<bool> MarkAsDeletedAsync(int id, string userId, CancellationToken ct = default)
    {
        var isSa = await IsSuperAdminAsync(userId, ct);
        var employee = await unitOfWork.Employees.GetByIdAsync(id);
        if (employee == null || employee.IsDeleted) return false;

        if (!isSa)
        {
            var leIds = await GetMyLegalEntityIdsAsync(userId, ct);
            if (!leIds.Contains(employee.LegalEntityId)) return false;
        }

        employee.IsDeleted = true;
        unitOfWork.Employees.Update(employee);
        await unitOfWork.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> RestoreAsync(int id, string userId, CancellationToken ct = default)
    {
        var isSa = await IsSuperAdminAsync(userId, ct);
        var employee = await unitOfWork.Employees.GetByIdAsync(id);
        if (employee == null || !employee.IsDeleted) return false;

        if (!isSa)
        {
            var leIds = await GetMyLegalEntityIdsAsync(userId, ct);
            if (!leIds.Contains(employee.LegalEntityId)) return false;
        }

        employee.IsDeleted = false;
        unitOfWork.Employees.Update(employee);
        await unitOfWork.SaveChangesAsync(ct);
        return true;
    }

    public async Task<PagedResult<EmployeeDto>> GetPagedAsync(int page, int pageSize, int? officeId, bool? isDeleted,
        string userId, CancellationToken ct = default)
    {
        var isSa = await IsSuperAdminAsync(userId, ct);
        var (items, _) = await unitOfWork.Employees.GetPagedAsync(page, pageSize, officeId, isDeleted);
        var list = items.ToList();

        if (!isSa)
        {
            var leIds = await GetMyLegalEntityIdsAsync(userId, ct);
            list = list.Where(e => leIds.Contains(e.LegalEntityId)).ToList();
        }

        var result = new PagedResult<EmployeeDto>
        {
            Items = list.Select(MapToDto).ToList(),
            Page = page,
            PageSize = pageSize,
            TotalCount = list.Count
        };
        return result;
    }

    public async Task<IEnumerable<EmployeeDto>> GetByOfficeAsync(int officeId, string userId,
        CancellationToken ct = default)
    {
        var isSa = await IsSuperAdminAsync(userId, ct);
        var items = (await unitOfWork.Employees.GetByOfficeAsync(officeId)).ToList();
        if (isSa) return items.Select(MapToDto);
        var leIds = await GetMyLegalEntityIdsAsync(userId, ct);
        return items.Where(e => leIds.Contains(e.LegalEntityId)).Select(MapToDto);
    }

    public Task<string> GeneratePasswordAsync(int length = 12)
    {
        const string lower = "abcdefghijklmnopqrstuvwxyz";
        const string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string digits = "0123456789";
        const string symbols = "!@#$%^&*()_-+=<>?";

        var all = lower + upper + digits + symbols;
        var rnd = new Random();

        var chars = new List<char>
        {
            lower[rnd.Next(lower.Length)],
            upper[rnd.Next(upper.Length)],
            digits[rnd.Next(digits.Length)],
            symbols[rnd.Next(symbols.Length)]
        };

        for (int i = chars.Count; i < length; i++)
            chars.Add(all[rnd.Next(all.Length)]);

        return Task.FromResult(new string(chars.OrderBy(_ => rnd.Next()).ToArray()));
    }


    private static EmployeeDto MapToDto(Employee e) => new()
    {
        Id = e.Id,
        OfficeId = e.OfficeId,
        OfficeName = e.Office.Name,
        LegalEntityId = e.LegalEntityId,
        LegalEntityName = e.LegalEntity.Name,
        Email = e.Email,
        FirstName = e.FirstName,
        LastName = e.LastName,
        MiddleName = e.MiddleName,
        Phone = e.PhoneNumber,
        IsDeleted = e.IsDeleted,
        LeadLimit = e.LeadLimit,
        CreatedAt = e.CreatedAt,
        Roles = e.UserRoles.Select(ur => ur.Role.Name).ToList(),
        RoleIds = e.UserRoles.Select(ur => ur.Role.Id).ToList(),
        Position = e.Position,
        PositionGenitive = e.PositionGenitive,
        PowerOfAttorneyNumber = e.PowerOfAttorneyNumber,
        LastNameGenitive = e.LastNameGenitive,
        FirstNameGenitive = e.FirstNameGenitive,
        MiddleNameGenitive = e.MiddleNameGenitive,
        MobilePhone = e.MobilePhone,
        AdditionalPhone = e.AdditionalPhone,
        BirthDate = e.BirthDate,
        TimeZone = e.TimeZone,
        ContactInfo = e.ContactInfo,
        HireDate = e.HireDate,
        SalaryAmount = e.SalaryAmount,
        WorkConditions = e.WorkConditions,
        Note = e.Note
    };
}