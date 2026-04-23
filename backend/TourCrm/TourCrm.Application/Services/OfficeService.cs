using TourCrm.Application.DTOs.Offices;
using TourCrm.Application.Interfaces;
using TourCrm.Core.Entities;
using TourCrm.Core.Interfaces;

namespace TourCrm.Application.Services;

public class OfficeService(
    IOfficeRepository officeRepo,
    ICompanyRepository companyRepo,
    ILegalEntityRepository legalRepo,
    IUnitOfWork uow
) : IOfficeService
{
    private const string SuperAdminRoleName = "SuperAdmin";

    private async Task<bool> IsSuperAdminAsync(string userId, CancellationToken ct)
    {
        if (!int.TryParse(userId, out var uid)) return false;
        var userRoles = (await uow.UserRoles.GetAllAsync(ct)).ToList();
        if (userRoles.Count == 0) return false;
        var roleIds = userRoles.Where(ur => ur.UserId == uid).Select(ur => ur.RoleId).ToHashSet();
        if (roleIds.Count == 0) return false;
        var roles = (await uow.Roles.GetAllAsync(ct)).ToList();
        return roles.Any(r =>
            roleIds.Contains(r.Id) && r.Name.Equals(SuperAdminRoleName, StringComparison.OrdinalIgnoreCase));
    }

    private async Task<int> GetMyCompanyIdOrThrowAsync(string userId, CancellationToken ct)
    {
        var company = await companyRepo.GetByOwnerAsync(userId, ct);
        if (company is not null) return company.Id;

        if (!int.TryParse(userId, out var uid))
            throw new InvalidOperationException("Некорректный идентификатор пользователя.");

        var me = await uow.Employees.GetByIdAsync(uid);
        if (me is null)
            throw new UnauthorizedAccessException("Пользователь не найден.");

        if (me.LegalEntityId <= 0)
            throw new InvalidOperationException("Пользователь не привязан к юрлицу.");

        var companyByLe = await companyRepo.GetByLegalEntityIdAsync(me.LegalEntityId, ct)
                          ?? throw new InvalidOperationException("Компания для указанного юрлица не найдена.");

        return companyByLe.Id;
    }

    public async Task<IReadOnlyList<OfficeListItemDto>> GetByLegalAsync(string userId, int legalEntityId, string? q,
        CancellationToken ct = default)
    {
        var isSa = await IsSuperAdminAsync(userId, ct);

        List<Office> offices;
        if (isSa)
        {
            var legalIds = (await legalRepo.GetAllAsync(ct))
                .Where(le => !le.IsDeleted)
                .Select(le => le.Id)
                .ToList();

            var acc = new List<Office>();
            foreach (var lid in legalIds)
            {
                var chunk = await officeRepo.GetByLegalAsync(lid, includeLegal: true, includeEmployees: true, ct: ct);
                acc.AddRange(chunk);
            }

            offices = acc.Where(o => !o.IsDeleted).ToList();
        }
        else
        {
            var le = await legalRepo.GetByIdAsync(legalEntityId, ct);
            if (le is null || le.IsDeleted) return Array.Empty<OfficeListItemDto>();

            var companyId = await GetMyCompanyIdOrThrowAsync(userId, ct);
            if (le.CompanyId != companyId) return Array.Empty<OfficeListItemDto>();

            offices = (await officeRepo.GetByLegalAsync(legalEntityId, includeLegal: true, includeEmployees: true,
                    ct: ct))
                .Where(o => !o.IsDeleted)
                .ToList();
        }

        var projected = offices.Select(o => new OfficeListItemDto
        {
            Id = o.Id,
            LegalEntityId = o.LegalEntityId,
            LegalEntityName = o.LegalEntity?.Name ?? o.LegalEntity?.NameRu ?? o.LegalEntity?.NameFull,
            Name = o.Name,
            Address = o.Address,
            Phone = o.Phone,
            Email = o.Email,
            LeadLimit = o.LeadLimit,
            EmployeesCount = o.Employees?.Count ?? 0,
            CreatedAt = o.CreatedAt
        }).ToList();

        if (string.IsNullOrWhiteSpace(q))
            return projected.OrderBy(x => x.Name).ToList();

        var qTrim = q.Trim();
        var needle = qTrim.ToLowerInvariant();
        var onlyDigits = new string(qTrim.Where(char.IsDigit).ToArray());
        int? empFilter = (onlyDigits.Length == qTrim.Length && int.TryParse(onlyDigits, out var n)) ? n : null;

        static string Digits(string? s) => new((s ?? string.Empty).Where(char.IsDigit).ToArray());

        var filtered = projected.Where(x =>
            (!string.IsNullOrEmpty(x.Name) && x.Name.ToLower().Contains(needle)) ||
            (!string.IsNullOrEmpty(x.Address) && x.Address!.ToLower().Contains(needle)) ||
            (!string.IsNullOrEmpty(x.LegalEntityName) && x.LegalEntityName!.ToLower().Contains(needle)) ||
            (!string.IsNullOrEmpty(x.Email) && x.Email!.ToLower().Contains(needle)) ||
            (!string.IsNullOrEmpty(x.Phone) && (x.Phone!.ToLower().Contains(needle) ||
                                                (!string.IsNullOrEmpty(onlyDigits) &&
                                                 Digits(x.Phone).Contains(onlyDigits)))) ||
            (empFilter.HasValue && x.EmployeesCount == empFilter.Value)
        );

        return filtered.OrderBy(x => x.Name).ToList();
    }


    public async Task<OfficeDto?> GetAsync(int id, string userId, CancellationToken ct = default)
    {
        var isSa = await IsSuperAdminAsync(userId, ct);

        var o = await officeRepo.GetByIdAsync(id, includeLegal: true, includeEmployees: false, ct: ct);
        if (o is null || o.IsDeleted) return null;

        if (!isSa)
        {
            var companyId = await GetMyCompanyIdOrThrowAsync(userId, ct);
            if (o.LegalEntity.CompanyId != companyId) return null;
        }

        return new OfficeDto
        {
            LegalEntityId = o.LegalEntityId,
            LegalEntityName = o.LegalEntity?.Name ?? o.LegalEntity?.NameRu ?? o.LegalEntity?.NameFull,
            Name = o.Name,
            Address = o.Address,
            Phone = o.Phone,
            Email = o.Email,
            LeadLimit = o.LeadLimit,
            CreatedAt = o.CreatedAt,
            UpdatedAt = o.UpdatedAt
        };
    }

    public async Task<OfficeDto> CreateAsync(OfficeUpsertDto dto, string userId, CancellationToken ct = default)
    {
        var isSa = await IsSuperAdminAsync(userId, ct);

        var le = await legalRepo.GetByIdAsync(dto.LegalEntityId, ct) ??
                 throw new InvalidOperationException("Юрлицо не найдено");
        if (le.IsDeleted) throw new UnauthorizedAccessException("Нет доступа к юрлицу.");

        if (!isSa)
        {
            var companyId = await GetMyCompanyIdOrThrowAsync(userId, ct);
            if (le.CompanyId != companyId) throw new UnauthorizedAccessException("Нет доступа к юрлицу.");
        }

        var o = new Office
        {
            LegalEntityId = dto.LegalEntityId,
            Name = dto.Name.Trim(),
            Address = dto.Address,
            Phone = dto.Phone,
            Email = dto.Email,
            LeadLimit = dto.LeadLimit,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        await officeRepo.AddAsync(o, ct);
        await uow.SaveChangesAsync(ct);

        return await GetAsync(o.Id, userId, ct) ?? new OfficeDto { };
    }

    public async Task<OfficeDto> UpdateAsync(int id, OfficeUpsertDto dto, string userId, CancellationToken ct = default)
    {
        var isSa = await IsSuperAdminAsync(userId, ct);

        var o = await officeRepo.GetByIdAsync(id, includeLegal: true, includeEmployees: false, ct: ct) ??
                throw new KeyNotFoundException("Офис не найден");
        if (o.IsDeleted) throw new UnauthorizedAccessException("Нет доступа к офису.");

        if (!isSa)
        {
            var companyId = await GetMyCompanyIdOrThrowAsync(userId, ct);
            if (o.LegalEntity.CompanyId != companyId) throw new UnauthorizedAccessException("Нет доступа к офису.");
        }

        if (o.LegalEntityId != dto.LegalEntityId)
        {
            var le = await legalRepo.GetByIdAsync(dto.LegalEntityId, ct) ??
                     throw new InvalidOperationException("Юрлицо не найдено");
            if (le.IsDeleted) throw new UnauthorizedAccessException("Нет доступа к юрлицу.");

            if (!isSa)
            {
                var companyId = await GetMyCompanyIdOrThrowAsync(userId, ct);
                if (le.CompanyId != companyId) throw new UnauthorizedAccessException("Нет доступа к юрлицу.");
            }

            o.LegalEntityId = dto.LegalEntityId;
        }

        o.Name = dto.Name.Trim();
        o.Address = dto.Address;
        o.Phone = dto.Phone;
        o.Email = dto.Email;
        o.LeadLimit = dto.LeadLimit;
        o.UpdatedAt = DateTime.UtcNow;

        officeRepo.Update(o);
        await uow.SaveChangesAsync(ct);

        return await GetAsync(o.Id, userId, ct) ?? new OfficeDto { };
    }

    public async Task SoftDeleteAsync(int id, string userId, CancellationToken ct = default)
    {
        var isSa = await IsSuperAdminAsync(userId, ct);

        var o = await officeRepo.GetByIdAsync(id, includeLegal: true, includeEmployees: false, ct: ct) ??
                throw new KeyNotFoundException("Офис не найден");
        if (o.IsDeleted) return;

        if (!isSa)
        {
            var companyId = await GetMyCompanyIdOrThrowAsync(userId, ct);
            if (o.LegalEntity.CompanyId != companyId) throw new UnauthorizedAccessException("Нет доступа к офису.");
        }

        o.IsDeleted = true;
        o.UpdatedAt = DateTime.UtcNow;

        officeRepo.Update(o);
        await uow.SaveChangesAsync(ct);
    }
}